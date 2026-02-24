Imports Microsoft.Xna.Framework

Namespace TowerDefense
    <Serializable>
    Public Class MapDataMessage
        Public Property Path As List(Of Vector2)
        Public Property PlayfieldSize As Vector2
        Public Property SidePanelWidth As Single
        Public Property Level As Integer
    End Class

    <Serializable>
    Public Class GameStateMessage
        Public Property Enemies As List(Of EnemyState)
        Public Property Bombs As List(Of BombState)
        Public Property HealthOrbs As List(Of HealthState)
        Public Property Projectiles As List(Of ProjectileState)
        Public Property Towers As List(Of TowerState)
        Public Property Level As Integer
        Public Property Currency As Integer
        Public Property EnemiesRemaining As Integer
        Public Property GameHealth As Integer
        Public Property MaxGameHealth As Integer
    End Class

    <Serializable>
    Public Class EnemyState
        Public Property Id As Guid
        Public Property Position As Vector2
        Public Property Radius As Single
        Public Property Health As Single
        Public Property MaxHealth As Single
        Public Property TypeBlend As Single
    End Class

    <Serializable>
    Public Class BombState
        Public Property Id As Guid
        Public Property Position As Vector2
        Public Property Radius As Single
        Public Property Countdown As Single
        Public Property ExplosionRadius As Single
    End Class

    <Serializable>
    Public Class HealthState
        Public Property Id As Guid
        Public Property Position As Vector2
        Public Property Radius As Single
        Public Property Amount As Single
    End Class

    <Serializable>
    Public Class ProjectileState
        Public Property Id As Guid
        Public Property Position As Vector2
        Public Property Radius As Single
        Public Property Velocity As Vector2
    End Class

    <Serializable>
    Public Class TowerState
        Public Property Id As Guid
        Public Property Position As Vector2
        Public Property TowerType As TowerType
    End Class

    <Serializable>
    Public Class PlaceTowerMessage
        Public Property Position As Vector2
        Public Property TowerType As TowerType
    End Class

    <Serializable>
    Public Class ClientReadyMessage
    End Class

    Public Enum TowerType
        Spiral
        Point
        Burst
        Rapid
        Sniper
    End Enum
End Namespace
