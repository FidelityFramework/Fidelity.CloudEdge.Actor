namespace Fidelity.CloudEdge.Actor

// Proposed vocabulary, not a wire schema or authority-minting API. A receiver
// must validate identities, scope and incarnation before accepting a command.
[<Struct>] type ActorId = ActorId of string
[<Struct>] type IncarnationId = IncarnationId of uint64
[<Struct>] type MessageId = MessageId of string
[<Struct>] type ConnectionId = ConnectionId of string
[<Struct>] type CheckpointId = CheckpointId of string
[<Struct>] type SchemaId = SchemaId of string
[<Struct>] type ArtifactId = ArtifactId of string
[<Struct>] type ReportSequence = ReportSequence of uint64
[<Struct>] type TopologyRevision = TopologyRevision of uint64

/// Logical routing identity and a particular supervised lifetime are distinct.
/// Hydration preserves this address; crash restart must not reuse its authority.
type ActorAddress = { Actor: ActorId; Incarnation: IncarnationId }

[<RequireQualifiedAccess>]
type ActorRole =
    | OlivierWorker
    | ProsperoSupervisor

/// Intended placement class, not evidence of an implemented provider or of the
/// requested placement having been granted. Ariel remains local to the host.
[<RequireQualifiedAccess>]
type HostingTarget =
    | DurableObject
    | DynamicWorker
    | Container
    | NativeClef

type Failure = { Code: string; Detail: string }
