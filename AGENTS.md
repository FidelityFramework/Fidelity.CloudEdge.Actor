# Actor library boundaries

- This repository is a contract scaffold. Do not describe declarations or planned
  adapters as an implemented actor runtime, durable inbox or recovery protocol.
- Keep portable actor contracts typed and functional: records, unions and explicit
  state/effects. No boxed semantic payloads, task-based domain APIs, reflection
  dispatch, ambient authority or callbacks serialized as actor state.
- Olivier owns actor behavior; Prospero owns supervision, lifecycle policy and
  placement; Ariel names local dispatch obligations. Cloudflare provides the host
  dispatch substrate. Do not equate this library with Cloudflare's Actors SDK.
- Preserve connection continuity through supported WebSocket hibernation while
  distinguishing it from durable message admission and processing completion.
- Dormancy/hydration preserve actor incarnation; crash restart must not silently
  revive old-incarnation authority. Logical routing identity is separate.
- Cancellation, timeout and lost connectivity do not prove remote work has stopped.
  Never manufacture cleanup acknowledgements or exactly-once external effects.
- BAREWire schemas and receiver validation define the boundary to native Clef
  actors. Fable object shapes and local correlation counters are not that schema.
- Keep platform bindings, storage transactions, clocks, JS promises and CLR interop
  in named future adapters. Do not import Incremental.Hosting into a Fable host.
- Compiler semantics/proofs remain Baker/PSG/Composer responsibilities. A lifecycle
  receipt or a scheduler eligibility decision cannot certify a native artifact.
- Use .NET/F#/JavaScript tooling; no Python automation. Keep raw evidence outside
  Git and clearly distinguish source checks, Fable compilation and workerd tests.
