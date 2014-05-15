Namespace Physics
    Public Interface PhysicalObject
        Inherits HellionEngine.Core.DisposableObject

        Sub OnCollision(Other As PhysicalObject)
    End Interface
End Namespace

