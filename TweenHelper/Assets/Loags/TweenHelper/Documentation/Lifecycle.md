# Lifecycle and option semantics

## Completion and kill

A finite tween completes when DOTween invokes its completion callback. A kill is a distinct terminal event and does not imply normal completion. `Kill(true)` may ask DOTween to complete before killing; consumers should still treat completion and kill callbacks as separate signals.

Infinite loops cannot complete normally. Keep their `TweenHandle` and explicitly kill or cancel them during owner teardown.

## Cancellation and timeout

`TweenAsync.AwaitCompletion` preserves existing callbacks. Cancelling its token kills the active tween and propagates `OperationCanceledException`.

`TweenAsync.AwaitCompletionWithTimeout` returns:

- `true` for normal completion;
- `false` for an external kill;
- `false` after the timeout, after killing the tween;
- an `OperationCanceledException` for cancellation from the caller's token.

## Builder option ownership

Each builder step owns a `TweenOptions` value.

- Before the first animation method, options apply to the next declared step.
- Immediately after an animation method, options update that declared step.
- After `Then()` or `With()`, options apply to the next declared step.
- `WithOptions` replaces the current step's options value.
- `WithEase`, `WithDelay`, `WithLoops`, and other individual modifiers update one property while preserving the rest.
- A method's explicit duration overrides `TweenOptions.Duration`; options duration overrides `TweenHelperSettings.DefaultDuration`.

## Target ownership

Built tweens are linked to their target GameObject. Destroying the target therefore kills the tween through DOTween's link behavior. Owners should still kill long-running or looping handles explicitly from their normal teardown path.
