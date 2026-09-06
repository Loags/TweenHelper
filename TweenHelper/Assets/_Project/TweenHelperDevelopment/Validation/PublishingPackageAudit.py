"""Audit a Unity-exported Tween Helper archive using Python's standard library."""
import argparse
import hashlib
import json
from pathlib import Path
import re
import tarfile


def audit(artifact, project):
    root = "Assets/Loags/TweenHelper"
    text_extensions = {".cs", ".md", ".meta", ".uxml", ".uss", ".asmdef", ".shader", ".asset", ".prefab", ".unity", ".mat"}
    paths = set()
    failures = []
    guids = set()
    with tarfile.open(artifact, "r:gz") as archive:
        members = {member.name: member for member in archive.getmembers()}
        for name, member in members.items():
            if not name.endswith("/pathname"):
                continue
            path = archive.extractfile(member).read().decode("utf-8").strip()
            if path in paths:
                failures.append("Duplicate path: " + path)
            paths.add(path)
            if path != root and not path.startswith(root + "/"):
                failures.append("Outside product root: " + path)
                continue
            if ".." in Path(path).parts:
                failures.append("Unsafe path: " + path)
                continue
            folder = name.rsplit("/", 1)[0]
            for suffix, local in (("asset", project / path), ("asset.meta", project / (path + ".meta"))):
                entry = members.get(folder + "/" + suffix)
                if entry is None:
                    if suffix == "asset.meta" or local.is_file():
                        failures.append("Missing archive member: " + str(local))
                    continue
                actual = archive.extractfile(entry).read()
                if not local.is_file():
                    failures.append("Missing source: " + str(local))
                    continue
                expected = local.read_bytes()
                if local.suffix in text_extensions:
                    actual = actual.replace(b"\r\n", b"\n")
                    expected = expected.replace(b"\r\n", b"\n")
                if actual != expected:
                    failures.append("Source mismatch: " + str(local))
                if suffix == "asset.meta":
                    match = re.search(rb"^guid: ([a-f0-9]{32})$", actual, re.M)
                    if not match or match[1] in guids:
                        failures.append("Invalid/duplicate GUID: " + path)
                    else:
                        guids.add(match[1])
        for local in (project / root).rglob("*"):
            if local.name.endswith(".meta"):
                continue
            relative = local.relative_to(project).as_posix()
            if relative not in paths:
                failures.append("Missing from archive: " + relative)
    return {"artifact": str(artifact), "sha256": hashlib.sha256(artifact.read_bytes()).hexdigest(), "bytes": artifact.stat().st_size, "pathCount": len(paths), "guidCount": len(guids), "failures": failures}


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("artifact", type=Path)
    parser.add_argument("--project", type=Path, default=Path("TweenHelper"))
    parser.add_argument("--output", type=Path)
    arguments = parser.parse_args()
    result = audit(arguments.artifact, arguments.project)
    rendered = json.dumps(result, indent=2)
    print(rendered)
    if arguments.output:
        arguments.output.write_text(rendered + "\n", encoding="utf-8")
    raise SystemExit(bool(result["failures"]))
