using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerPositionSyncer : UdonSharpBehaviour
{
    // The place to store the player's position and rotation
    [UdonSynced]
    private Vector3 _Position;
    [UdonSynced]
    private Quaternion _Rotation;

    // The player that this script is attached to
    private VRCPlayerApi _owner;

    // Delay interval for updating position
    private const float UpdateInterval = 0.5f;

    /// <summary>
    /// If the data is restored, teleport the player to the restored position.
    /// </summary>
    /// <param name="player">The player whose data is restored.</param>
    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        player.TeleportTo(_Position, _Rotation);
    }

    private void Start()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            _owner = Networking.LocalPlayer;
            SendCustomEventDelayedSeconds(nameof(UpdatePosition), UpdateInterval);
        }
    }

    /// <summary>
    /// Get the player's position and rotation every 0.5 seconds and store it in the player data.
    /// </summary>
    public void UpdatePosition()
    {
        if (_owner != null && _owner.IsPlayerGrounded()) // Only update the position if the player is grounded
        {
            _Position = _owner.GetPosition();
            _Rotation = _owner.GetRotation();
            RequestSerialization();
        }
        SendCustomEventDelayedSeconds(nameof(UpdatePosition), UpdateInterval);
    }
}