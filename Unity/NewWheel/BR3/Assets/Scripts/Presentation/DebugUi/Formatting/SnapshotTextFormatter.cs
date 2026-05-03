using BR3.Domain.Results;

namespace BR3.Presentation.DebugUi
{
    public static class SnapshotTextFormatter
    {
        public static string Format(PhaseSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return "-";
            }

            return
                $"Phase: {snapshot.Phase}\n" +
                $"Enemy Lane: {snapshot.EnemyLaneStateText}\n" +
                $"Player Lane: {snapshot.PlayerLaneStateText}";
        }
    }
}
