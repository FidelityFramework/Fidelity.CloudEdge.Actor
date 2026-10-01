# Acceptance plan

This is a prioritized plan for a contract scaffold, not evidence of an actor runtime,
durable inbox, native integration or deployed recovery protocol. Each gate requires its
own recorded result; passing a later-looking source check cannot substitute for execution.

Olivier owns worker behavior; Prospero owns supervision, lifecycle policy and placement.
Cloudflare supplies execution and dispatch. The Cloudflare realization introduces no
independent Ariel scheduler, remote turn dispatcher or general actor SDK foundation.
The [scheduler contract §7](https://forge.spkez.dev/FidelityFramework/clef-lang-spec/src/commit/746ef16232164a5ad2620a535b9fb521705f4226/spec/scheduler-contract.md)
requires an assumption manifest and explicit refusal of unsupported guarantees.

The [Incremental functional checkpoint](https://forge.spkez.dev/FidelityFramework/Fidelity.FSharp.Incremental/src/commit/613e2600c1f1696eb0a363b18d8ecdd7c9b2764b/docs/Functional_Async_Checkpoint_2026-10-01.md)
supplies local ownership lessons, not distributed acceptance: admission is distinct from
acknowledgement; a lost observer does not retract admitted work; cancellation is a request;
physical cleanup remains owned after faults. Do not import its CLR Hosting implementation.

## Ordered gates

| Priority | Gate | Required evidence and limit |
| --- | --- | --- |
| 0 | Contract/scaffold compilation | Typed lifecycle, state, reporting and resumption declarations compile with their declared dependencies. Descriptive target cases imply no provider or execution capability. This demonstrates no actor execution. |
| 1 | Portable transition and Fable traces | Scripted arrivals, time, completions and faults produce identical accepted commands, state transitions and effects in reference and Fable runs. Neither successful Fable compilation nor one trace establishes hosted behavior. |
| 2 | Local workerd adapter | Real storage, reconnect, transaction failure and process interruption exercise the receipt/state protocol. Pin bindings, Fable/runtime versions and configuration. Local restart does not demonstrate hosted WebSocket hibernation. |
| 3 | Hosted lifecycle and hibernation | Actual hosted activation/hibernation receipts establish connection continuity, durable recovery and incarnation handling under named platform assumptions. Record unavailable observations as limits. |
| 4 | Parent/worker topology | One Prospero parent supervises a bounded Olivier child through a supported Dynamic Worker, durable facet, container or external-worker adapter. Admit only the capabilities actually implemented by that adapter. |
| 5 | Native BAREWire boundary | Independent encoded fixtures and a real native peer validate agreed schema, identity, payload fidelity and receiver refusal. Fable object shapes and local counters are not wire compatibility or compiler artifact authority. |

## First vertical: one durable actor and one owned operation

Start with one bounded actor inbox, one typed operation, one state update and a receipt
lookup. Keep external effects out of this first vertical. Retain exact logical routing,
incarnation, operation and attempt identities, code/schema compatibility and payload
identity. Reject conflicting reuse rather than silently interpreting an old ID anew.

- Interrupt before durable commit, after commit but before acknowledgement, and after
  acknowledgement but before reply observation. An accepted acknowledgement must refer
  to committed request/state evidence. A lost reply reconciles that same operation.
- Replay the same ID with identical payload, then conflicting payload. Repeat after
  reconstruction. Require an explicit expiry/replay-horizon rule; deleted deduplication
  history must not silently turn an expired operation into fresh work.
  After crash restart, retrieving a prior receipt must neither reapply its state update
  nor grant that old incarnation authority to resume or launch new work.
- Hold an observer, cancel or disconnect it, then reconnect and observe the exact receipt.
  Disconnect does not establish actor death or remote completion.
- Inject storage failure and callback reentrancy around transitions. No partial state,
  duplicate acknowledgement or observer callback may escape an uncommitted transition.
- Record rejection of malformed, stale-incarnation and incompatible-schema requests.
  Source declarations alone do not prove receiver validation was executed.

## Lifecycle, state and resumption

Test dormancy/hydration separately from crash restart. The proposed dormant-mailbox
doorbell is a control event; message data remains in the actor's bounded inbox. Hydration
preserves incarnation; crash restart issues new incarnation authority. Constructor entry
alone cannot identify either event. If the host cannot distinguish them adequately, the
adapter must leave this gate unresolved rather than revive an old reference implicitly.

Persist typed control position, immutable environment, current dependencies and operation
identity before promising durable resumption. Do not persist closures or live resources.
Exercise duplicate resume, conflicting response, old attempt, old schema/code, and changed
state dependencies. Each stale case must refuse before behavior or external effects run.
An ordinary immutable response token does not register or prove those dependencies.

For supported hosted sockets, demonstrate a still-connected client crossing actual
hibernation and receiving the resumed operation's receipt. Separately test connection loss
and reattachment, including a cancelled observer while owned work remains suspended.
Attachments and a live socket are not substitutes for durable actor state.
See [Durable Object hosting](Durable_Object_Hosting.md) for the proposed host boundaries.

## Supervision, awareness and dynamic workers

Future parent APIs should request child creation, placement, retirement, cancellation,
status and reconciliation through typed operations. They must not dispatch a remote turn.
Pin parent/child identities, topology version, deployment capability and execution owner.
Test spawn/reply loss, duplicate creation, reparenting, parent restart and delayed reports.
An old topology snapshot, progress report or successful health report cannot restore lease,
result or lifecycle authority. Report unknown ownership distinctly from confirmed drain.

Hold a remote worker and its cleanup after cancellation, timeout and parent disconnect.
Keep ownership unresolved until the execution owner's actual receipt or an explicit
recovery policy settles it; reject late results independently of physical termination.
Fork a lease and resume both holders: the receiver/effect destination must enforce fencing
at the actual commit boundary, not merely when a request was queued. An external effect
needs destination idempotency or reconciliation; local durable state is not exactly-once I/O.

Test pending-work, payload, connection and retry budgets as well as inbox capacity.
Saturating data traffic must not consume reserved control admission for retirement,
reference updates and declared supervision actions. Receiver refusal must not become an
unbounded sender retry buffer. Non-yielding host code is not interruptible merely because
control messages have reserved capacity; publish the substrate's limitation explicitly.

Awareness/reporting is bounded and non-authoritative. Saturate and stop the telemetry
consumer; recording loss must not block lifecycle progress or create scheduler traffic.
Expose incarnation, state/topology version, observed sequence and uncertainty in reports.
Keep diagnostic history and causal receipts distinct from authorization to launch work.

## Evidence required to advance

For each gate retain source/tool/configuration pins, exact scenario and fault schedule,
state/effect traces, receipt identities, expected refusals and cleanup outcome externally.
Use intentional mutations to check critical refusal and ordering oracles for vacuous
checks. State which properties are discharged locally, checked at a boundary or assumed
from the host. Unsupported targets must remain unexecuted, with explicit refusal evidence.

Before native execution, compare independent BAREWire encoders/decoders, truncated and
wrong-layout messages, numeric fidelity, foreign incarnation and stale operation handling.
Compiler acceptance remains Baker/PSG/Composer's responsibility. Actor completion or
scheduler eligibility cannot certify a proof receipt, artifact or execution target.
