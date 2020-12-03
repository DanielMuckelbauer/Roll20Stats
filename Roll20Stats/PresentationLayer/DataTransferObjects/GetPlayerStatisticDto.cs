﻿namespace Roll20Stats.PresentationLayer.DataTransferObjects
{
    public class GetPlayerStatisticDto : RequestWithErrors
    {
        public string CharacterId { get; set; }
        public string CharacterName { get; set; }
        public int DamageDealt { get; set; }
        public int DamageTaken { get; set; }
    }
}