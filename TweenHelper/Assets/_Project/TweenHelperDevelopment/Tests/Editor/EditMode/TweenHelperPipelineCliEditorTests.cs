using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LB.TweenHelper.Automation.Editor;
using LB.TweenHelper.Pipeline.Editor;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Unity.Pipeline.Editor;
using Unity.Pipeline.Commands;
using Unity.Pipeline.Editor.Authoring;
using Unity.Pipeline.Security;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace LB.TweenHelper.Tests.Editor
{
    public sealed class TweenHelperPipelineCliEditorTests
    {
        private static readonly string[] ExpectedCommandIds =
        {
            "tween_helper_catalog",
            "tween_helper_context",
            "tween_helper_describe_operation",
            "tween_helper_dev_contract_probe",
            "tween_helper_setup_status",
            "tween_helper_target_profile"
        };

        [Test]
        public void BuiltInCatalog_FreshBuildHasExactly300Descriptors_WithoutChangingRegistry()
        {
            RegistrySnapshot before = RegistrySnapshot.Capture();
            MethodInfo buildMethod = typeof(BuiltInPresetCatalog).GetMethod("Build", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.IsNotNull(buildMethod);
            var catalog = (BuiltInPresetCatalog)buildMethod.Invoke(null, null);

            Assert.IsNotNull(catalog);
            Assert.IsEmpty(catalog.Issues);
            Assert.AreEqual(TweenHelperAutomationContract.BuiltInPresetCount, catalog.Operations.Count);
            Assert.AreEqual(catalog.Operations.Count, catalog.Operations.Select(operation => operation.Id).Distinct(StringComparer.Ordinal).Count());
            Assert.That(catalog.CatalogHash, Does.Match("^sha256:[0-9a-f]{64}$"));
            Assert.IsTrue(catalog.Operations.All(HasCompleteDescriptor));
            before.AssertUnchanged();
        }

        [Test]
        public void Commands_AreUniqueAndGenerateOneStructuredInputSchema()
        {
            var commands = CommandRegistry.DiscoverCommands().ToArray();
            foreach (string commandId in ExpectedCommandIds)
            {
                CommandInfo[] matches = commands.Where(command => command.Name == commandId).ToArray();
                Assert.AreEqual(1, matches.Length, $"Expected exactly one discovered command named '{commandId}'.");
                Assert.AreEqual(1, matches[0].Parameters.Count);
                Assert.AreEqual("input", matches[0].Parameters[0].Name);
                Assert.IsTrue(matches[0].Parameters[0].Required);
                Assert.IsTrue(matches[0].MainThreadRequired);
            }

            CommandInfo probeCommand = commands.Single(command => command.Name == "tween_helper_dev_contract_probe");
            JObject schema = JObject.Parse(JsonSchemaGenerator.GenerateCommandSchema(probeCommand));
            JToken input = schema["properties"]?["input"];
            Assert.AreEqual("object", (string)input?["type"]);
            Assert.AreEqual(false, (bool?)input?["additionalProperties"]);
            Assert.AreEqual("object", (string)input?["properties"]?["objectReference"]?["type"]);
            Assert.AreEqual("object", (string)input?["properties"]?["vector"]?["type"]);
            Assert.AreEqual("object", (string)input?["properties"]?["color"]?["type"]);
            Assert.AreEqual(false, (bool?)input?["properties"]?["objectReference"]?["additionalProperties"]);
            Assert.AreEqual(false, (bool?)input?["properties"]?["vector"]?["additionalProperties"]);
            Assert.AreEqual(false, (bool?)input?["properties"]?["color"]?["additionalProperties"]);
        }

        [Test]
        public void CatalogAndDescribe_DirectInvocation_ReturnStructuredBodies()
        {
            JObject catalog = TweenHelperPipelineCommands.Catalog(new CatalogInput
            {
                SchemaVersion = TweenHelperAutomationContract.SchemaVersion,
                RequestId = "test.direct.catalog",
                PageSize = 2
            });

            Assert.AreEqual("valid", (string)catalog["status"]);
            Assert.AreEqual(TweenHelperAutomationContract.BuiltInPresetCount, (int)catalog["data"]?["builtInCount"]);
            Assert.AreEqual(2, ((JArray)catalog["data"]?["operations"]).Count);
            Assert.AreEqual("test.direct.catalog", (string)catalog["requestId"]);
            string cursor = (string)catalog["data"]?["nextCursor"];
            string operationId = (string)catalog["data"]?["operations"]?[0]?["id"];

            JObject nextPage = TweenHelperPipelineCommands.Catalog(new CatalogInput
            {
                SchemaVersion = TweenHelperAutomationContract.SchemaVersion,
                PageSize = 2,
                Cursor = cursor
            });
            string[] firstPageIds = ((JArray)catalog["data"]?["operations"]).Select(operation => (string)operation["id"]).ToArray();
            string[] secondPageIds = ((JArray)nextPage["data"]?["operations"]).Select(operation => (string)operation["id"]).ToArray();
            Assert.AreEqual("valid", (string)nextPage["status"]);
            Assert.IsFalse(firstPageIds.Intersect(secondPageIds, StringComparer.Ordinal).Any());

            JObject reboundCursor = TweenHelperPipelineCommands.Catalog(new CatalogInput
            {
                SchemaVersion = TweenHelperAutomationContract.SchemaVersion,
                Query = "fade",
                PageSize = 2,
                Cursor = cursor
            });
            Assert.AreEqual("invalid", (string)reboundCursor["status"]);
            Assert.AreEqual("invalid_cursor", (string)reboundCursor["errors"]?[0]?["code"]);

            JObject description = TweenHelperPipelineCommands.DescribeOperation(new DescribeOperationInput
            {
                SchemaVersion = TweenHelperAutomationContract.SchemaVersion,
                OperationId = operationId
            });

            Assert.AreEqual("valid", (string)description["status"]);
            Assert.AreEqual(operationId, (string)description["data"]?["operation"]?["id"]);
            Assert.IsNotEmpty((string)description["data"]?["operation"]?["verificationOracle"]);
            Assert.IsNotEmpty((string)description["data"]?["catalogHash"]);
        }

        [Test]
        public void SetupStatus_ReportsPinnedCompatibilityTuple()
        {
            JObject setup = TweenHelperPipelineCommands.SetupStatus(new SetupStatusInput
            {
                SchemaVersion = TweenHelperAutomationContract.SchemaVersion
            });

            JToken compatibility = setup["data"]?["compatibility"];
            Assert.AreEqual("ready", (string)setup["status"]);
            Assert.AreEqual("1.0.0", (string)compatibility?["tweenHelperVersion"]);
            Assert.AreEqual(Application.unityVersion, (string)compatibility?["unityVersion"]);
            Assert.AreEqual("0.3.1-exp.1", (string)compatibility?["pipelineVersion"]);
            Assert.AreEqual("1.3.030", (string)compatibility?["dotweenRuntimeVersion"]);
            Assert.AreEqual("2.5.0", (string)compatibility?["uguiVersion"]);
            Assert.AreEqual("2.5.0", (string)compatibility?["textMeshProVersion"]);
            Assert.AreEqual("com.unity.ugui", (string)compatibility?["textMeshProPackageId"]);
            Assert.AreEqual("1.7.0", (string)compatibility?["testFrameworkVersion"]);
        }

        [Test]
        public void Context_ReturnsSanitizedReadOnlyShape()
        {
            JObject context = TweenHelperPipelineCommands.Context(new ContextInput
            {
                SchemaVersion = TweenHelperAutomationContract.SchemaVersion
            });

            JToken data = context["data"];
            JToken scene = data?["scene"];
            Assert.AreEqual("ready", (string)context["status"]);
            Assert.IsNull(scene?["name"]);
            Assert.IsNull(scene?["path"]);
            Assert.IsNull(data?["projectName"]);
            Assert.IsNull(data?["projectPath"]);
            Assert.AreEqual(false, (bool?)data?["capabilities"]?["persistentAuthoring"]);

            string[] allowlist = { "Asset", "CanvasGroup", "Component", "GameObject", "Graphic", "RectTransform", "Renderer", "SceneAsset", "SpriteRenderer", "TMP_Text", "Transform", "UnityObject" };
            Assert.IsTrue(((JArray)data?["selection"]?["objectTypes"]).All(type => allowlist.Contains((string)type, StringComparer.Ordinal)));
        }

        [Test]
        public void Catalog_ViaIsolatedPipelineServer_PreservesLiveDescriptorAndRunInBackground()
        {
            bool originalRunInBackground = Application.runInBackground;
            DescriptorSnapshot descriptorBefore = DescriptorSnapshot.Capture();
            try
            {
                using (var server = new IsolatedPipelineServer())
                {
                    server.Start();
                    if (descriptorBefore.Exists) Assert.AreNotEqual(descriptorBefore.Port, server.Port);
                    JObject response = server.Execute("tween_helper_catalog", new JObject
                    {
                        ["input"] = new JObject
                        {
                            ["schemaVersion"] = TweenHelperAutomationContract.SchemaVersion,
                            ["requestId"] = "test.isolated.catalog",
                            ["pageSize"] = 1
                        }
                    });

                    Assert.AreEqual(true, (bool?)response["success"]);
                    JObject result = response["result"] as JObject;
                    Assert.IsNotNull(result);
                    Assert.AreEqual("valid", (string)result["status"]);
                    Assert.AreEqual(TweenHelperAutomationContract.BuiltInPresetCount, (int)result["data"]?["builtInCount"]);
                    Assert.AreEqual(1, ((JArray)result["data"]?["operations"]).Count);
                }
            }
            finally
            {
                Application.runInBackground = originalRunInBackground;
            }

            Assert.AreEqual(originalRunInBackground, Application.runInBackground);
            descriptorBefore.AssertLiveDescriptorUnchanged();
        }

        [Test]
        public void TargetProfile_ExplicitInstanceId_DoesNotMutateProjectOrRegistry()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            bool activeSceneDirty = activeScene.IsValid() && activeScene.isDirty;
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            bool? prefabStageDirty = prefabStage == null ? null : prefabStage.scene.isDirty;
            Object[] selection = Selection.objects.ToArray();
            string[] assetPaths = AssetDatabase.GetAllAssetPaths().OrderBy(path => path, StringComparer.Ordinal).ToArray();
            string manifestHash = ComputeFileHash("Packages/manifest.json");
            string projectSettingsHash = ComputeFileHash("ProjectSettings/ProjectSettings.asset");
            RegistrySnapshot registry = RegistrySnapshot.Capture();
            Scene previewScene = EditorSceneManager.NewPreviewScene();
            var target = new GameObject("TweenHelperPipelineReadOnlyFixture") { hideFlags = HideFlags.HideAndDontSave };
            SceneManager.MoveGameObjectToScene(target, previewScene);

            try
            {
                var identity = ObjectResolver.Describe(target);
                Assert.IsNotNull(identity);
                Assert.IsTrue(identity.InstanceId.HasValue);
                bool previewSceneDirty = previewScene.isDirty;
                bool targetDirty = EditorUtility.IsDirty(target);
                Vector3 localPosition = target.transform.localPosition;
                Quaternion localRotation = target.transform.localRotation;
                Vector3 localScale = target.transform.localScale;
                int componentCount = target.GetComponents<Component>().Length;

                JObject result = TweenHelperPipelineCommands.TargetProfile(new TargetProfileInput
                {
                    SchemaVersion = TweenHelperAutomationContract.SchemaVersion,
                    RequestId = "test.target.profile",
                    Target = new ObjectReferenceInput { InstanceId = identity.InstanceId.Value.ToString() },
                    CompatiblePageSize = 3
                });

                Assert.AreEqual("valid", (string)result["status"]);
                Assert.That((string)result["data"]?["targetProfileHash"], Does.Match("^sha256:[0-9a-f]{64}$"));
                Assert.AreEqual(3, ((JArray)result["data"]?["compatibleOperationIds"]).Count);
                Assert.IsNull(result["data"]?["identity"]?["hierarchyPath"]);
                Assert.AreEqual(previewSceneDirty, previewScene.isDirty);
                Assert.AreEqual(targetDirty, EditorUtility.IsDirty(target));
                Assert.AreEqual(localPosition, target.transform.localPosition);
                Assert.AreEqual(localRotation, target.transform.localRotation);
                Assert.AreEqual(localScale, target.transform.localScale);
                Assert.AreEqual(componentCount, target.GetComponents<Component>().Length);
                Assert.AreEqual(activeSceneDirty, activeScene.IsValid() && activeScene.isDirty);
                Assert.AreEqual(prefabStageDirty, prefabStage == null ? null : prefabStage.scene.isDirty);
                CollectionAssert.AreEqual(selection, Selection.objects);
                CollectionAssert.AreEqual(assetPaths, AssetDatabase.GetAllAssetPaths().OrderBy(path => path, StringComparer.Ordinal).ToArray());
                Assert.AreEqual(manifestHash, ComputeFileHash("Packages/manifest.json"));
                Assert.AreEqual(projectSettingsHash, ComputeFileHash("ProjectSettings/ProjectSettings.asset"));
                registry.AssertUnchanged();
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(previewScene);
            }
        }

        [Test]
        public void TargetProfileHash_StableGlobalIdExcludesVolatileInstanceId()
        {
            Scene previewScene = EditorSceneManager.NewPreviewScene();
            var target = new GameObject("TweenHelperPipelineHashFixture") { hideFlags = HideFlags.HideAndDontSave };
            SceneManager.MoveGameObjectToScene(target, previewScene);

            try
            {
                var firstIdentity = new TargetIdentity
                {
                    GlobalId = "GlobalObjectId_V1-2-aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa-123-0",
                    InstanceId = "100",
                    ObjectType = "GameObject"
                };
                var secondIdentity = new TargetIdentity
                {
                    GlobalId = firstIdentity.GlobalId,
                    InstanceId = "200",
                    ObjectType = firstIdentity.ObjectType
                };

                ServiceResult<TargetProfileData> first = TargetProfileService.Profile(target, new TargetProfileRequest { Identity = firstIdentity, CompatiblePageSize = 1 });
                ServiceResult<TargetProfileData> second = TargetProfileService.Profile(target, new TargetProfileRequest { Identity = secondIdentity, CompatiblePageSize = 1 });

                Assert.AreEqual("valid", first.Status);
                Assert.AreEqual("valid", second.Status);
                Assert.AreEqual(first.Data.TargetProfileHash, second.Data.TargetProfileHash);
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(previewScene);
            }
        }

        [Test]
        public void ObjectReference_MultipleAddressForms_ReturnsHandledInvalidResult()
        {
            JObject result = TweenHelperPipelineCommands.TargetProfile(new TargetProfileInput
            {
                SchemaVersion = TweenHelperAutomationContract.SchemaVersion,
                Target = new ObjectReferenceInput
                {
                    GlobalId = "GlobalObjectId_V1-0-0-0-0",
                    InstanceId = "1"
                }
            });

            Assert.AreEqual("invalid", (string)result["status"]);
            Assert.AreEqual("invalid_object_reference", (string)result["errors"]?[0]?["code"]);
        }

        [Test]
        public void ContractProbe_MissingRequiredVectorComponent_ReturnsHandledInvalidResult()
        {
            JObject result = TweenHelperPipelineCommands.ContractProbe(new ContractProbeInput
            {
                SchemaVersion = TweenHelperAutomationContract.SchemaVersion,
                ObjectReference = new ObjectReferenceInput { Path = "Assets/SchemaProbe.asset" },
                Vector = new Vector3Input { Y = 2f, Z = 3f },
                Color = new ColorInput { R = 1f, G = 1f, B = 1f, A = 1f }
            });

            Assert.AreEqual("invalid", (string)result["status"]);
            Assert.IsTrue(((JArray)result["errors"]).Any(issue => (string)issue["code"] == "missing_required_field" && (string)issue["fieldPath"] == "input.vector.x"));
        }

        [Test]
        public void CanonicalJson_IsCultureInvariantAndMatchesGoldenHash()
        {
            CultureInfo originalCulture = CultureInfo.CurrentCulture;
            CultureInfo originalUiCulture = CultureInfo.CurrentUICulture;
            try
            {
                string[] cultures = { "en-US", "de-DE", "tr-TR" };
                string[] hashes = cultures.Select(cultureName =>
                {
                    CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                    return ComputeCanonicalFixtureHash();
                }).ToArray();

                Assert.AreEqual(1, hashes.Distinct(StringComparer.Ordinal).Count());
                Assert.IsTrue(hashes.All(hash => hash == "sha256:1f40a233078941f92a60e67c5e87b81d9b81d511f6d60019e4687bc5d01b6986"));
                Assert.Throws<ArgumentOutOfRangeException>(() => CanonicalHash.Compute(writer => writer.WriteSingle(float.NaN)));
                Assert.Throws<ArgumentOutOfRangeException>(() => CanonicalHash.Compute(writer => writer.WriteDouble(double.PositiveInfinity)));
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
                CultureInfo.CurrentUICulture = originalUiCulture;
            }
        }

        private static bool HasCompleteDescriptor(OperationDescriptor descriptor)
        {
            return !string.IsNullOrEmpty(descriptor.Id) &&
                   !string.IsNullOrEmpty(descriptor.PresetName) &&
                   descriptor.DefaultDuration > 0f &&
                   descriptor.TargetRequirements.Length > 0 &&
                   descriptor.OptionAllowlist.Length > 0 &&
                   descriptor.MutationFootprint.Length > 0 &&
                   !string.IsNullOrEmpty(descriptor.Determinism) &&
                   !string.IsNullOrEmpty(descriptor.VerificationOracle);
        }

        private static string ComputeCanonicalFixtureHash()
        {
            return CanonicalHash.Compute(writer =>
            {
                writer.BeginObject();
                writer.WritePropertyName("float");
                writer.WriteSingle(1.25f);
                writer.WritePropertyName("negativeZero");
                writer.WriteSingle(-0f);
                writer.WritePropertyName("null");
                writer.WriteNull();
                writer.WritePropertyName("text");
                writer.WriteString("Cafe\u0301\n");
                writer.EndObject();
            });
        }

        private static string ComputeFileHash(string projectRelativePath)
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), projectRelativePath.Replace('/', Path.DirectorySeparatorChar));
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] digest = sha256.ComputeHash(File.ReadAllBytes(fullPath));
                var result = new StringBuilder(digest.Length * 2);
                foreach (byte value in digest) result.Append(value.ToString("x2", CultureInfo.InvariantCulture));
                return result.ToString();
            }
        }

        private sealed class DescriptorSnapshot
        {
            public bool Exists { get; private set; }
            public int ProcessId { get; private set; }
            public int Port { get; private set; }
            public string Mode { get; private set; }

            public static DescriptorSnapshot Capture()
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Library", "Pipeline", ".unity-pipeline-port");
                if (!File.Exists(path)) return new DescriptorSnapshot();
                JObject descriptor = JObject.Parse(File.ReadAllText(path));
                return new DescriptorSnapshot
                {
                    Exists = true,
                    ProcessId = (int)descriptor["pid"],
                    Port = (int)descriptor["port"],
                    Mode = (string)descriptor["mode"]
                };
            }

            public void AssertLiveDescriptorUnchanged()
            {
                DescriptorSnapshot current = Capture();
                Assert.AreEqual(Exists, current.Exists);
                if (!Exists) return;
                Assert.AreEqual(ProcessId, current.ProcessId);
                Assert.AreEqual(Port, current.Port);
                Assert.AreEqual(Mode, current.Mode);
            }
        }

        private sealed class IsolatedPipelineServer : EditorPipelineServer, IDisposable
        {
            protected override bool WritesDescriptor => false;

            protected override (int basePort, int maxPort) GetPortRange() => (7850, 7899);

            protected override string GetToken() => SecurityTokenManager.GetOrCreateToken();

            protected override object GetServerStatus() => new { status = "ready", lastHeartbeat = DateTime.UtcNow };

            protected override void ServerStarted()
            {
            }

            public JObject Execute(string command, JObject parameters, int timeoutMilliseconds = 30000)
            {
                Task<JObject> request = Task.Run(() => Post(command, parameters));
                DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);
                while (!request.IsCompleted)
                {
                    Dispatcher.ProcessWorkQueue();
                    if (DateTime.UtcNow > deadline) throw new TimeoutException($"Command '{command}' did not complete within {timeoutMilliseconds}ms.");
                    Thread.Sleep(1);
                }

                return request.GetAwaiter().GetResult();
            }

            public void Dispose() => Stop();

            private async Task<JObject> Post(string command, JObject parameters)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                    var payload = new JObject
                    {
                        ["command"] = command,
                        ["parameters"] = parameters ?? new JObject()
                    };
                    using (var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json"))
                    using (HttpResponseMessage response = await client.PostAsync($"http://127.0.0.1:{Port}/api/exec", content).ConfigureAwait(false))
                    {
                        string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        if (!response.IsSuccessStatusCode) throw new InvalidOperationException($"Pipeline returned HTTP {(int)response.StatusCode}: {body}");
                        return JObject.Parse(body);
                    }
                }
            }
        }

        private sealed class RegistrySnapshot
        {
            private bool HasScannedCodePresets { get; set; }
            private int RegistryVersion { get; set; }
            private string[] PresetsByName { get; set; }
            private string[] PresetsByType { get; set; }

            public static RegistrySnapshot Capture()
            {
                Type registryType = typeof(TweenPresetRegistry);
                return new RegistrySnapshot
                {
                    HasScannedCodePresets = (bool)GetField(registryType, "_hasScannedCodePresets").GetValue(null),
                    RegistryVersion = (int)GetField(registryType, "_registryVersion").GetValue(null),
                    PresetsByName = CaptureDictionary(GetField(registryType, "_presetsByName").GetValue(null)),
                    PresetsByType = CaptureDictionary(GetField(registryType, "_presetsByType").GetValue(null))
                };
            }

            public void AssertUnchanged()
            {
                RegistrySnapshot current = Capture();
                Assert.AreEqual(HasScannedCodePresets, current.HasScannedCodePresets);
                Assert.AreEqual(RegistryVersion, current.RegistryVersion);
                CollectionAssert.AreEqual(PresetsByName, current.PresetsByName);
                CollectionAssert.AreEqual(PresetsByType, current.PresetsByType);
            }

            private static FieldInfo GetField(Type type, string name)
            {
                FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Static);
                Assert.IsNotNull(field, $"Expected TweenPresetRegistry field '{name}'.");
                return field;
            }

            private static string[] CaptureDictionary(object dictionary)
            {
                var entries = new System.Collections.Generic.List<string>();
                IDictionaryEnumerator enumerator = ((IDictionary)dictionary).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    entries.Add($"{enumerator.Key}|{RuntimeHelpers.GetHashCode(enumerator.Value)}");
                }

                entries.Sort(StringComparer.Ordinal);
                return entries.ToArray();
            }
        }
    }
}
