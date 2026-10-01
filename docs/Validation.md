# Initial scaffold validation — 2026-10-01

This checkpoint establishes documentation and a compilable contract vocabulary.
It does not establish an actor implementation. No Durable Object was deployed,
no existing service was replaced, and no compiler or Incremental checkpoint changed.

| Check | Result and scope |
| --- | --- |
| .NET Release solution build | Passed, zero warnings/errors; SDK 10.0.401, netstandard2.0, FSharp.Core 10.1.401. |
| Fable JavaScript compilation | Passed with Fable 5.13.0; all three source files compiled. Generated output is outside Git. |
| Semantic/runtime tests | Not run: no transition engine, inbox, codec or hosting provider exists to exercise yet. |
| workerd or hosted Cloudflare | Not run for this library. Sibling project's historical probes are supporting context only. |
| Native Clef/BAREWire interoperability | Not implemented or tested. |
| Packaging/publication | Disabled in this scaffold; source repository publication is separate. |

The sole non-framework source dependency is FSharp.Core. The .NET Standard build
also resolves NETStandard.Library 2.0.3 and Microsoft.NETCore.Platforms 1.1.0.
There is no dependency on Cloudflare's Actors SDK, Incremental.Hosting, IcedTasks,
FSharp.Data.Adaptive or the generated CloudEdge package collection. Future adapters
will declare their specific dependencies when their execution contracts are built.

Build and generated-source evidence is external at
`/home/hhh/.codex/work/cloudedge-actor-scaffold-2026-10-01/`:
`build.log`, `tool-restore.log`, `fable.log`, `source.sha256` and generated output.
The tested pre-commit assembly SHA256 is
`68929e9dbc45c24afd5784b86380b399b30056d6022b67cfa3fc1cabbdc82075`.

| Generated module | SHA256 |
| --- | --- |
| Identity.js | `3a3729001f74151e2adfb9a97320861ec42a8d4610574dd905b2968e5f60b973` |
| Lifecycle.js | `8ac524512399575561231e202017546163bccfe38bf90155e9761f1ef347e36b` |
| Protocol.js | `81a6f0eb44064d89d6358cd51d6373a0f7e0d140dc9384442f00d1a5ac94f0f4` |

Compilation verifies the present declarations and toolchain, not serialization,
durable reconstruction, fairness, supervision or equality of execution traces.
The [acceptance plan](Acceptance_Plan.md) identifies those later gates.
The first documentation link check caught the not-yet-written validation document.
The completed set passed all 16 relative-link checks across eight Markdown files;
the staged repository also passed whitespace checks.

[Clef actor alignment](Clef_Actor_Alignment.md) records pinned specification/site
sources and fresh LAN repository discovery. A bounded LAN worker review informed
one acceptance scenario concerning old receipts after crash restart; its other
suggestions were independently filtered. The request, response and disposition are
external at `/home/hhh/.codex/work/cloudedge-actor-2026-10-01/acceptance-review/`.
Neither retrieval nor worker review constitutes runtime acceptance.

The build used a shared Bozzetto lease, which was released after compilation.
This repository does not change a service, model context setting, live compiler
pin or another consumer's dependency versions.
