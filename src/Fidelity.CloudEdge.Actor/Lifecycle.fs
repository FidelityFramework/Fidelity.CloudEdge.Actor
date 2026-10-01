namespace Fidelity.CloudEdge.Actor

[<RequireQualifiedAccess>]
type Termination =
    | Completed
    | Stopped
    | Failed of Failure

/// Descriptions only: this scaffold does not implement lifecycle transitions.
/// Dormancy is not termination, and Stopping is not cleanup acknowledgement.
[<RequireQualifiedAccess>]
type ActorLifecycle =
    | Initializing
    | Active
    | Dormant
    | Hydrating
    | Stopping
    | Terminated of Termination

[<RequireQualifiedAccess>]
type ConnectionMode =
    | OrdinaryWebSocket
    | HibernationWebSocket

[<RequireQualifiedAccess>]
type ConnectionStatus =
    | Connected
    | Disconnected

/// Connection mode and actor activation are orthogonal. A hibernation-managed
/// connection may remain connected while the actor's execution is dormant.
type ConnectionReport = {
    Connection: ConnectionId
    Mode: ConnectionMode
    Status: ConnectionStatus
}

/// A connectivity observation is not a termination or cleanup certificate.
[<RequireQualifiedAccess>]
type Reachability =
    | Reachable
    | Unreachable
    | Unknown

type ActorDescriptor = {
    Address: ActorAddress
    Role: ActorRole
    Target: HostingTarget
}

[<RequireQualifiedAccess>]
type RelationKind =
    | ParentOf
    | Supervises
    | RoutesTo

type TopologyRelation = {
    Source: ActorAddress
    Target: ActorAddress
    Kind: RelationKind
}

/// Descriptive topology. Admission, cycle rules and change authorization belong
/// to a future owner; publishing this value does not create those relationships.
type TopologySnapshot = {
    Revision: TopologyRevision
    Actors: ActorDescriptor list
    Relations: TopologyRelation list
}

type ActorReport = {
    Actor: ActorAddress
    Lifecycle: ActorLifecycle
    Reachability: Reachability
    Connections: ConnectionReport list
}

/// Sequence and topology context let a consumer detect stale observations once
/// authenticated owner/ordering rules exist. Construction is not attestation.
type Observation<'Report> = {
    Reporter: ActorAddress
    Sequence: ReportSequence
    Topology: TopologyRevision
    Value: 'Report
}
