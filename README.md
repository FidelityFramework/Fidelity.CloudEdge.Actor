# Fidelity.CloudEdge.Actor

A typed, functional actor contract scaffold for FSharp.CloudEdge, Cloudflare
Durable Objects and BAREWire-connected native Clef actors.

**Status: design and declarations only.** The portable library compiles for .NET
and through Fable to JavaScript. It does not yet run actors, persist an inbox,
recover state, spawn workers or establish compiler/platform conformance.

Olivier supplies the actor behavior model; Prospero supplies supervision, lifecycle
policy and placement. Ariel names local dispatch obligations. On Cloudflare,
dispatch is supplied by the platform. Cloudflare's Actors SDK is not the semantic
foundation of this library.

The intended durable inbox is a logical service that can remain addressable while
an actor is dormant. Supported WebSocket hibernation preserves accepted connections
while the Durable Object leaves memory; durable admission, state and result receipts
make accepted work recoverable. Connection continuity, persisted acceptance and
actual execution completion are separate parts of that contract.

The initial vocabulary covers:

- Logical actor identity and incarnation; dormancy versus termination.
- Typed messages, admission and result receipts, and versioned state checkpoints.
- Connection continuity, reachability, lifecycle reports and versioned topology.
- Parent-facing child specifications for prospective Durable Object, Dynamic Worker,
  container and native Clef placements.

Declarations describe the proposed API vocabulary. They do not validate a received
message, mint authority, define a wire encoding or implement any hosting provider.
The initial source depends only on FSharp.Core beyond .NET Standard framework references. Future adapters can use
Xantham-generated FSharp.CloudEdge bindings without placing SDK objects or CLR Tasks
in the portable semantic model.

Start with the [documentation index](docs/README.md) and
[architecture](docs/Architecture.md). The [acceptance plan](docs/Acceptance_Plan.md)
separates scaffold compilation from functional, durable, hosted and native gates.
[Validation](docs/Validation.md) records exactly what has been checked.

```sh
dotnet build Fidelity.CloudEdge.Actor.slnx -c Release
dotnet tool restore
dotnet fable src/Fidelity.CloudEdge.Actor/Fidelity.CloudEdge.Actor.fsproj --outDir artifacts/fable --lang JavaScript
```

SDK 10.0.401 and Fable 5.13.0 are pinned. Generated output is ignored; package
publication is disabled while the contracts remain a scaffold. MIT licensed.
The primary repository is [Fidelity.CloudEdge.Actor on Forgejo](https://forge.spkez.dev/FidelityFramework/Fidelity.CloudEdge.Actor).
