# Alignment with Clef actor design

This repository is a contract scaffold for a future CloudEdge actor library.
Its semantic direction comes from Clef's actor design, not Cloudflare's Actors SDK.
No declaration here establishes an implemented runtime, durable mailbox, recovery
protocol, native compiler integration, or scheduler conformance.

## Authority and component ownership

The pinned **normative specification** governs the ownership split. Site articles
explain the design and propose extensions; illustrative APIs are not shipped contracts.
The choices described below as **bridge policy** are this scaffold's proposed
interpretation, not additional claims about the specification or Cloudflare.

| Owner | Responsibility | Boundary for this library |
|---|---|---|
| Olivier | Actor behavior, private state, message processing and actor semantics | Typed behavior, lifecycle and message contracts; no untyped semantic payload store |
| Prospero | Supervision, restart policy, placement, retirement and cross-domain deadlines/escalation | Parent/child policy and explicit lifecycle decisions |
| Ariel | Local dispatch and delivery of each resumption under the scheduler contract | No user-facing scheduler API; no remote turn dispatch |
| Cloudflare host | Platform execution, dispatch, storage and supported lifecycle facilities | Named adapters and explicit host assumptions |

[Scheduler Contract §§1–8][S1] makes these assignments and explicitly says that
Cloudflare orchestration does not introduce an independent Ariel scheduler.
The scheduler's clients are the supervisor and compiler. A parent scheduler may
grant a subordinate a budget and region; it does not dispatch the subordinate's turns.
Network-crossing policy belongs to supervision, while local dispatch stays local.

An actor's turn ends at suspension, completion or fault. Single-threaded JavaScript
does not make an asynchronous handler atomic, establish fairness, or guarantee
that supervision can interrupt a callback that never yields. The future adapter
must state its reentrancy, admission, control-capacity and progress assumptions.
Unsupported requirements remain explicit findings. Older site's simplified edge
tables must not override these normative isolate qualifications. [S1]

## Parents, dynamic workers and topology

[Unified Actor Architecture][D2] presents parents spawning children, monitoring
failures, applying bounded restart policies and escalating to their own parents.
It also describes peer channels independently of the supervision tree. These are
design precedents; its sample Worker Loader calls and WebSocket monitoring do not
establish an implementation or a reliable remote-failure detector in this library.

The proposed bridge keeps three relationships distinct: supervision ancestry,
message-routing connections, and job/partition membership. Placement is Prospero
policy constrained by supported host capabilities. A logical child may require a
new execution incarnation after failure without changing its routing name.
Connection loss establishes an observation requiring policy, not proof of death.

Dynamic child creation must have explicit ownership and accounting. For an
all-results job, establish a closed expected partition set or a justified protocol
that closes it; a zero counter alone is insufficient. Acceptance must associate
the current input/job version and partition with its contribution, reject or
reconcile conflicts, and leave state unchanged for duplicates. Persist accepted
identities and contributions consistently if recovery promises to preserve them.
Quorum, streaming and first-success joins require their own contracts. [D3]

## Dormancy, lifecycle and resumption

The sleeping-actor discussion in [Surfacing the Scheduler][D1] explicitly proposes
`Dormant`, a bounded mailbox that exists before actor activation, and a
control-plane admission notification that asks Prospero to arrange hydration.
The message remains in the receiver's mailbox; the supervisor receives the
notification rather than becoming another data queue.

This scaffold adopts the proposal as **bridge policy**: dormancy/hydration preserves
the actor incarnation; crash restart establishes a fresh incarnation and must not
silently restore authority held by old references. Logical routing identity,
host Durable Object identity, execution incarnation and job version are separate
concepts. The native sentinel vocabulary is design context, not an implemented
CloudEdge reference protocol. This policy needs a concrete host realization.

Supported WebSocket hibernation may preserve connections while application memory
is absent. It does not by itself establish durable message admission, processing
completion or restorable continuation state. Receipt acknowledgement, remote
cancellation acknowledgement and physical cleanup remain distinct obligations.
Timeout, cancellation request and lost connectivity never certify remote cleanup.

The normative [continuation representation][S2] records the suspension index,
in-flight value, live captures and internal state; VC-ONE requires exactly one
resumption of a suspended frame. Compiler elaboration and saturation settle this
structure. A future library adapter must preserve the contract rather than infer
capture validity or repair source semantics at runtime.

For remote or durable resumption, [D3] additionally requires correspondence to the
pending logical operation, valid input/state premises, no prior completion or
supersession, and actor-state exclusivity when execution resumes. Its durable
continuation record is a **proposal**: compatible code/control position, retained
observations, job/partition state and failure/cancellation state, with local
resources rebound or rejected. Persisting a CLR or V8 closure is not that contract.

## Native and Clef direction

F# records, unions, opaque handles and Async workflows can provide an executable
reference for a later Clef surface. [Fully Informed Bindings][D4] describes this as
a proposed compatibility path: shared contracts, schema-directed narrowing and
DCont-backed operations, with independent evidence for each target realization.
Cloudflare declarations and wrapper SDKs supply interoperability facts, not the
semantic foundation. No source-language proof follows from a generated binding.

BAREWire is the intended shared boundary with native Clef actors. Agree schemas,
version/session interpretation, identity fields and rejection behavior explicitly.
Decoded shape is not proof that a remote actor computed the right result, and
reply deduplication cannot undo repeated external effects. [D3]

The [incremental specification §1.2][S3] permits many incremental nodes inside
one actor; it does not equate each node with an actor or define distributed
stabilization. A future adapter may reuse explicit local command/effect laws from
Fidelity.FSharp.Incremental. Its in-process handles and cleanup receipts do not
establish remote admission, durable recovery or supervisor policy.

Baker/CCS retains semantic analysis, capture/lifetime settlement and proof premises;
PSG publishes settled structure, Alex composes passively, and Composer owns target
realization. This scaffold establishes neither compiler completion nor platform
conformance. Future acceptance must separately exercise lifecycle transitions,
stale/duplicate replies, saturation, restart recovery and actual cleanup.

## Source provenance

Reviewed 2026-10-01 from local `clef-lang-spec` and `clef-lang-site` checkouts.
All seven primary source files below matched their recorded HEAD revisions.
Spec revision: `746ef16232164a5ad2620a535b9fb521705f4226`.
Site revision: `f5efe4b15eb71ebede9aca2cad5499dd910f89ae`.

| Source | Status and inspected scope | SHA256 |
|---|---|---|
| [S1: Scheduler Contract][S1] | Normative; §§1–10 | `092c8b3521fd2ccc40372cca42843b329c8e1fb33a6756143d88cfd28bf46f65` |
| [S2: Delimited Continuation Representation][S2] | Normative; §§1–7 | `e5953c9425b59eaf410980d6b184efbdf860395c82d3b87a25818ce0d4f0a96f` |
| [S3: Incremental Computation][S3] | Normative; §§1–1.3 | `4a061abf7efb0329bd01a8f3b35c2fde44c4eaaf8c043deeed172ac0bf15bb9c` |
| [D1: Surfacing the Scheduler][D1] | Site design; ownership and sleeping-actor proposal | `74687fd72f038e7d5c2293b8d8effc364db6dea7bf94f596f1a46c53e6cc10d3` |
| [D2: Unified Actor Architecture][D2] | Site design; shared abstraction, supervision, topology and state | `b09e17e6c2bb465929f0c1b471f8e2c2c8a477e069ce568043eb04749f570e4b` |
| [D3: Proof Preservation Across Actors and Workflows][D3] | Site design; execution domains through recovery and open premises | `9fb0006073bc775aacb1188fff4a4e50f5ae64907d5e42116a5ed45d98e826be` |
| [D4: Fully Informed Bindings][D4] | Proposed binding and compatibility path | `ca68fc7bc9b653148acbf66ef05e09b34a7e51751c2377fc08fdf67189ff80f3` |

Bounded LAN repository discovery independently located S1 and D1 at those revisions
under fresh snapshot `30f3a9fe9c28e1757e757bf2b748d6d125869309093c1c5355087e7f53a547f7`
(generation 3). Two initial queries omitted the required snapshot and were rejected;
schema refresh and explicit-snapshot queries succeeded. This was literal repository
discovery, not semantic proof or hybrid ranking. Direct pinned text governs this note.
No runtime, compiler, Fable or workerd checks were executed for this document.

[S1]: https://forge.spkez.dev/FidelityFramework/clef-lang-spec/src/commit/746ef16232164a5ad2620a535b9fb521705f4226/spec/scheduler-contract.md
[S2]: https://forge.spkez.dev/FidelityFramework/clef-lang-spec/src/commit/746ef16232164a5ad2620a535b9fb521705f4226/spec/dcont-representation.md
[S3]: https://forge.spkez.dev/FidelityFramework/clef-lang-spec/src/commit/746ef16232164a5ad2620a535b9fb521705f4226/spec/incremental-computation.md
[D1]: https://forge.spkez.dev/FidelityFramework/clef-lang-site/src/commit/f5efe4b15eb71ebede9aca2cad5499dd910f89ae/hugo/content/blog/surfacing-the-scheduler.md
[D2]: https://forge.spkez.dev/FidelityFramework/clef-lang-site/src/commit/f5efe4b15eb71ebede9aca2cad5499dd910f89ae/hugo/content/blog/unified-actor-architecture.md
[D3]: https://forge.spkez.dev/FidelityFramework/clef-lang-site/src/commit/f5efe4b15eb71ebede9aca2cad5499dd910f89ae/hugo/content/docs/design/javascript-targeting/proof-preservation-across-actors-and-workflows.md
[D4]: https://forge.spkez.dev/FidelityFramework/clef-lang-site/src/commit/f5efe4b15eb71ebede9aca2cad5499dd910f89ae/hugo/content/docs/design/javascript-targeting/fully-informed-bindings.md
