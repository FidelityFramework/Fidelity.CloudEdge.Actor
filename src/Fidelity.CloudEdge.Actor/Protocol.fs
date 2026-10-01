namespace Fidelity.CloudEdge.Actor

/// Typed message data, not an executable continuation. Sender and destination
/// are untrusted claims until the receiver validates them at admission.
type Envelope<'Message> = {
    Id: MessageId
    Destination: ActorAddress
    Sender: ActorAddress option
    Correlation: MessageId option
    Payload: 'Message
}

/// The durable store must bind this identity to the exact accepted message and
/// destination incarnation. This record alone is not proof of that commit.
type AdmissionReceipt = { Message: MessageId; Destination: ActorAddress }

[<RequireQualifiedAccess>]
type AdmissionFailure =
    | WrongIncarnation
    | IdentityConflict
    | CapacityExceeded
    | Closing
    | Unauthorized
    | UnsupportedCapability of string

/// Persisted admission is separate from execution/result completion. Duplicate
/// acceptance must refer to the same committed identity and payload binding.
[<RequireQualifiedAccess>]
type AdmissionOutcome =
    | Accepted of AdmissionReceipt
    | AlreadyAccepted of AdmissionReceipt
    | Rejected of AdmissionFailure

[<RequireQualifiedAccess>]
type ProcessingOutcome<'Reply> =
    | Succeeded of 'Reply
    | Failed of Failure
    | Cancelled

/// An operation outcome does not imply that its owned remote work has drained.
type ResultReceipt<'Reply> = {
    Admission: AdmissionReceipt
    Outcome: ProcessingOutcome<'Reply>
}

/// Intended recovery description. A future codec/store must persist the schema,
/// typed state and all authority/dependency data required by that schema.
/// This is not a serializer or a restorable Incremental.Core snapshot.
type Checkpoint<'State> = {
    Id: CheckpointId
    Actor: ActorAddress
    Schema: SchemaId
    State: 'State
}

/// Parent-facing intent only. No provider, spawn function, supervision policy or
/// readiness acknowledgement is implemented by this placeholder.
type ChildSpecification<'Configuration> = {
    Request: MessageId
    Parent: ActorAddress
    Child: ActorId
    Role: ActorRole
    Target: HostingTarget
    Artifact: ArtifactId
    Configuration: 'Configuration
}
