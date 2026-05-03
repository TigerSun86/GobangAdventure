using System;

namespace BR3.Presentation.DebugUi
{
    [Serializable]
    public sealed class DebugUiState
    {
        public int SelectedSnapshotPhaseIndex;
        public string StatusMessage;

        public static DebugUiState CreateDefault()
        {
            return new DebugUiState
            {
                SelectedSnapshotPhaseIndex = 0,
                StatusMessage = "Assign a config asset and click Load Config.",
            };
        }
    }
}
