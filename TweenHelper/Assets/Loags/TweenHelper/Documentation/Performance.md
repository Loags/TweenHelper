# Performance and ownership

Create animations when an interaction starts, not from every Update call. A retained recipe handle can rewind/restart the same sequence; rebuilding also repeats binding validation and tween construction. Kill owned handles on teardown. Avoid restarting two mesh effects on the same TMP label without stopping the previous owner.

## Representative CPU measurements

The 1.3.0 candidate was measured in non-development Windows x64 Mono and IL2CPP players using Unity 6000.5.2f1 on an Intel i7-6700K at 4 GHz, Windows 10.0.19045, Built-in pipeline. IL2CPP used High managed stripping. These were headless CPU benchmarks with a null graphics device and no attached profiler, not FPS or rendering benchmarks. The machine was also running other applications. Each workload ran one warmup followed by 120 manual updates at a simulated 60 Hz. Recorded steps include Tween Helper/DOTween animation work, not a rendered game frame.

| Workload | Mono median / p95 (ms) | IL2CPP median / p95 (ms) |
| --- | --- | --- |
| 500 transform tweens | 0.110 / 0.156 | 0.085 / 0.137 |
| 500-target collection recipe | 0.087 / 0.111 | 0.034 / 0.037 |
| 256-glyph rich TMP wave | 0.254 / 0.382 | 0.086 / 0.090 |
| 1,024-glyph rich TMP wave | 1.047 / 1.289 | 0.260 / 0.439 |

All workloads returned to their starting active-tween count after cleanup. One hundred create/wait/cancel cycles completed in 13.66 ms (Mono) and 8.61 ms (IL2CPP). Explicit registry refresh took 7.07 ms and 8.16 ms respectively. Backend differences here are observations from one busy host, not guaranteed speedups.

These measurements establish a profiling baseline, not minimum hardware requirements or universal budgets. Start by profiling 10/100/500 collection targets and 32/256/1,024 visible glyphs in your own rendered scene. Long TMP effects were the largest measured steady animation cost here; restrict them to visible content or animate smaller text groups when your frame budget requires it.

## Allocations and rendering limits

The runtime's per-thread allocation counter reported zero for a known 4 KB allocation in both players. The validation reports therefore mark allocation measurements unsupported (-1), not allocation-free. Builders, tasks, cancellation registrations, collection bindings and TMP state buffers can allocate. Use the Unity Profiler in a development build to measure managed allocations and rendering costs on your target device.

Text construction, layout and glyph changes can rebuild TMP mesh state. Changing a label's text during an effect may require regeneration; prefer stable contents while animating and profile dynamic text separately. Large UI collections also incur Canvas/layout work that this headless benchmark does not measure. Consider staggering creation across interactions and reuse authored UI objects where appropriate.

Reduced motion changes presentation, not computational complexity. It preserves timing and lifecycle work, and does not promise a performance improvement. Mobile, WebGL, GPU rendering and additional Unity versions need their own measurements.
