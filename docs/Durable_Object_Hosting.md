# Durable Object hosting

Design checkpoint: **2026-10-01**. This repository contains contract scaffolding.
The hosting adapters, durable inbox, recovery protocol and remote execution providers
described below are future work. No deployment or hosted hibernation test is claimed.

## Actor continuity through hibernation

Connection continuity is part of the intended actor semantics. A Durable Object using
Cloudflare's Hibernation WebSocket API can leave memory while its accepted clients remain
connected to Cloudflare. An incoming event reconstructs the object and invokes its handler.
The actor remains reachable through that connection without requiring a continuously
resident actor instance. Server connections enter this model through `ctx.acceptWebSocket`;
the conventional WebSocket API and outgoing WebSockets have different lifecycle behavior.
[Cloudflare WebSockets guide](https://developers.cloudflare.com/durable-objects/best-practices/websockets/)
(updated 2026-09-30; checked 2026-10-01).

Connection attachments can restore small session metadata across hibernation while the
connection remains healthy. Closing the connection loses those attachments; durable actor
records belong in storage. Hibernation preserves supported connections, while connection
failure and platform shutdown still require reconnect handling.
[Attachment behavior](https://developers.cloudflare.com/durable-objects/best-practices/websockets/#websocketserializeattachment).

Cloudflare reruns constructors after activation and discards ordinary in-memory state.
Pending handlers, I/O and timers affect whether an object can hibernate. Deployments and
runtime shutdown are additional lifecycle events, and reliable pre-shutdown hooks are not
provided. Persist recovery information during operation instead of relying on a finalizer.
[Durable Object lifecycle](https://developers.cloudflare.com/durable-objects/concepts/durable-object-lifecycle/)
(updated 2026-09-30; checked 2026-10-01).

The proposed library separates logical routing identity, actor incarnation and activation.
Dormancy followed by hydration preserves the incarnation. Crash restart cannot silently
restore authority from an earlier incarnation. Constructor execution alone does not settle
which transition occurred. A future adapter must establish how persisted records and host
observations distinguish these cases; this scaffold has no such recovery implementation.

## Durable service and message receipts

The intended actor inbox is a persistent logical service. A process-local F# mailbox is one
possible dispatch mechanism during an activation. Its queued values, waiter continuations
and running workflows do not become durable merely because the enclosing host is a Durable
Object. This distinction preserves the useful connection continuity described above.

The future delivery protocol must distinguish these observations:

| Observation | Proposed meaning |
| --- | --- |
| Frame received | The transport delivered bytes for decoding and validation. |
| Request accepted | Capacity was reserved and the exact request was committed durably. |
| Result produced | The execution owner reported the operation outcome. |
| Owned work drained | Actual owned execution and cleanup were acknowledged complete. |
| Result applied | The actor validated current authority and committed the state transition. |
| Reply observed | The requester received a receipt referring to the exact request. |

Durable Objects provide storage and transaction primitives. Storage gates address storage
ordering, while JavaScript execution can interleave across asynchronous calls; the future
adapter must preserve the actor's chosen command ordering explicitly.
[SQLite storage](https://developers.cloudflare.com/durable-objects/api/sqlite-storage-api/)
(checked 2026-10-01).
Initialization can use `blockConcurrencyWhile`, whose failures or timeout can reset the
object. It is unsuitable as an indefinite wait for remote worker cleanup.
[State API](https://developers.cloudflare.com/durable-objects/api/state/#blockconcurrencywhile)
(checked 2026-10-01).

Proposed admission commits the request identity, validated payload identity and acceptance
receipt together. Retrying an identical identity retrieves that operation; reusing it with
different payload data is a protocol refusal. A requester that loses its reply queries or
retries the same identity, including after reconnect or hydration. It does not infer that
the original request was rejected. Receipt retention and any eventual expiry must be
explicit so expired history cannot accidentally authorize duplicate execution.

For external effects, a future durable outbox records intent and correlates the target's
receipt. Storage atomicity alone cannot make an external service execute exactly once.
Retries require a target idempotency or reconciliation contract. Cloudflare likewise makes
retry advice conditional on idempotent requests and documents replacing failed stubs.
[Error handling](https://developers.cloudflare.com/durable-objects/best-practices/error-handling/)
(updated 2026-04-21; checked 2026-10-01).

## Placement and execution choices

Olivier owns actor behavior. Prospero owns supervision, lifecycle policy and placement.
Ariel names local dispatch obligations; Cloudflare supplies the host dispatch substrate.
The following are candidate placements, with no provider implemented in this repository.

| Host component | Intended responsibility and boundary |
| --- | --- |
| Durable Object supervisor | Stable routing, durable actor records, acceptance and result validation, supported inbound connections. |
| Dynamic Worker | Run selected Workers-compatible code with explicit bindings and network access. Its cached isolate is execution machinery. |
| Durable Object facet | A dynamically loaded durable child supervised by a Durable Object, with its own SQLite storage. |
| Container | Execute a Linux process or toolchain through the controlling Durable Object. Process ownership and actor ownership remain explicit. |
| External worker | Execute elsewhere under a correlated request and report progress, results and cleanup through the actor protocol. |

The Worker Loader API is now documented under **Dynamic Workers**. `get` may reuse an
isolate, but subsequent calls through the same stub can execute in different isolates;
its identifier must select consistent code/configuration. The identifier is therefore not
evidence of actor incarnation or exclusive execution ownership.
[Worker Loader API](https://developers.cloudflare.com/dynamic-workers/api-reference/)
(updated 2026-09-10; checked 2026-10-01).

Dynamic Workers also support Durable Object facets: the supervisor obtains a facet by
name, with isolated SQLite storage and explicit abort/delete operations. Aborting preserves
storage; deleting removes it. This is a possible durable child topology, not a reason to
assume a plain dynamically loaded Worker has persistent actor state. Facet placement and
supervision need their own acceptance tests.
[Durable Object facets](https://developers.cloudflare.com/dynamic-workers/usage/durable-object-facets/)
(checked 2026-10-01).

Containers are accessed through a controlling Durable Object. Startup, readiness, exit and
storage have separate contracts: `running` precedes readiness, `monitor` observes process
exit, and disk is ephemeral by default. Durable Object and container placement need not be
the same location. Candidate native Clef execution still requires a compatible artifact,
image, protocol and evidence; the container topology does not certify any of those.
[Container lifecycle](https://developers.cloudflare.com/containers/concepts/architecture/)
(updated 2026-09-30; checked 2026-10-01).

## Remote ownership and local dispatch

Each future remote launch needs an exact request, actor incarnation, attempt identity,
execution owner and declared input/environment identity. Admission is separate from the
authority checked at the eventual external effect boundary. Persisted suspension contains
typed data and the next protocol step, never a JavaScript closure or a captured CLR stack.
BAREWire schemas and receiver validation define the boundary to native actors.

Cancellation requests withdrawal; timeout or a lost connection makes ownership uncertain.
Neither certifies that a worker stopped or completed cleanup. After a host reset, reconcile
the exact attempt with its execution owner. A superseded generation can refuse late result
application without proving the old process is dead. Replacement policy must address that
remaining physical ownership and any external side effects. These are future protocol laws.

The Incremental hosting work supplies a useful local discipline: cancellation of an
observer detaches that observer, while admitted work and awaited close retain ownership.
Completion joins evaluator children and cancellation callbacks; a protocol fault does not
invent cleanup acknowledgements. Distributed recovery needs explicit evidence for the same
distinctions. A process-local join cannot survive reset as an in-memory continuation.
[Incremental checkpoint](https://forge.spkez.dev/FidelityFramework/Fidelity.FSharp.Incremental/src/commit/613e2600c1f1696eb0a363b18d8ecdd7c9b2764b/docs/Functional_Async_Checkpoint_2026-10-01.md).

The inspected Fable 5.13.0 runtime also requires deliberate scheduling: `Async.Start` runs
immediately and mailbox `Post` can invoke a waiting continuation inline. The future adapter
must provide the intended deferred dispatch, avoid observer execution inside state changes,
and test callback/cleanup failures. Importing Incremental.Hosting would retain CLR-specific
assumptions. Persist a waiting state and return from a handler when the design expects the
actor to hibernate; an indefinitely awaited remote operation has different lifecycle costs.
The inspected runtime is Fable 5.13.0's `Async.js` and `MailboxProcessor.js`,
present in CloudEdge's generated ByteBridge output. The public
[Fable compatibility guide](https://fable.io/docs/javascript/compatibility.html)
also describes the supported Async/mailbox surface; it does not establish this
library's execution behavior.

## Existing evidence and the next acceptance boundary

Local sibling sources were inspected on 2026-10-01. CloudEdge's
[ChatRoom example](https://forge.spkez.dev/FidelityFramework/FSharp.CloudEdge/src/commit/3bf08618837910e433b779cf6c0396fc900a1080/site/examples/DurableObjects/ChatRoom.fs)
accepts hibernating sockets and supplies message/close handlers. Its
[Durable Object test guide](https://forge.spkez.dev/FidelityFramework/FSharp.CloudEdge/src/commit/3bf08618837910e433b779cf6c0396fc900a1080/docs/durable-objects-testing.md)
records bounded local workerd checks for duplicate receipts, transactions, alarm retry and
process restart, while explicitly leaving hosted hibernation and distributed delivery open.
Those bindings and checks do not implement this repository's actor policies.

Future acceptance must exercise: connection continuity across actual hibernation; durable
acceptance followed by a lost reply; reconnect and receipt lookup; duplicate/payload-conflict
refusal; hydration versus crash incarnation handling; a remote worker that outlives its
observer; stale reports after replacement; and close held pending for real child/callback
cleanup. Record Fable compilation, local workerd execution and hosted platform behavior as
separate evidence. Source inspection and this design document establish none of those runs.
