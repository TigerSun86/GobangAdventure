using BR3.Domain.Runtime;

namespace BR3.Presentation.DebugUi
{
    public static class BoardSlotTextFormatter
    {
        public static BoardSlotViewData Format(BoardSlotState slotState, int? currentRoundIndex = null)
        {
            int slotIndex = slotState?.Index + 1 ?? 0;

            if (slotState == null)
            {
                return new BoardSlotViewData
                {
                    SlotTitleText = "Slot -",
                    OccupantNameText = "-",
                    TraitsText = "Traits: -",
                    PowerText = "Power: -",
                    ExtraText = "-",
                };
            }

            if (!slotState.IsOpen)
            {
                return new BoardSlotViewData
                {
                    SlotTitleText = $"Slot {slotIndex}",
                    OccupantNameText = "Closed",
                    TraitsText = "Traits: -",
                    PowerText = "Power: -",
                    ExtraText = "Closed slot",
                };
            }

            if (slotState.Occupant == null)
            {
                return new BoardSlotViewData
                {
                    SlotTitleText = $"Slot {slotIndex}",
                    OccupantNameText = "Open",
                    TraitsText = "Traits: None",
                    PowerText = "Power: -",
                    ExtraText = "Open and empty",
                };
            }

            BoardCard occupant = slotState.Occupant;
            string extraText = "-";
            if (occupant.EnterRoundIndex > 0)
            {
                extraText = currentRoundIndex.HasValue && occupant.EnterRoundIndex == currentRoundIndex.Value
                    ? $"New this round: {occupant.EnterRoundIndex}"
                    : $"Entered: {occupant.EnterRoundIndex}";
            }

            return new BoardSlotViewData
            {
                SlotTitleText = $"Slot {slotIndex}",
                OccupantNameText = CardTextFormatter.FormatTitle(occupant.SourceCard),
                TraitsText = CardTextFormatter.FormatTraits(occupant.SourceCard),
                PowerText = $"Power: {occupant.CurrentPower}",
                ExtraText = extraText,
            };
        }
    }
}
