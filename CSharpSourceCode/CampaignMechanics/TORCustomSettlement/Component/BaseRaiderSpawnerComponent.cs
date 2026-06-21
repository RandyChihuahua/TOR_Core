using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TOR_Core.Extensions;

namespace TOR_Core.CampaignMechanics.TORCustomSettlement.Component;

public abstract class BaseRaiderSpawnerComponent : TORBaseSettlementComponent
{
    protected static string CONFIG_NODE_BATTLE_SCENE = "battle_scene";

    protected static string CONFIG_NODE_RAIDER_TEMPLATE = "raider_template";

    protected static string CONFIG_NODE_RAIDER_PARTY_NAME = "raider_party_name";

    protected static string CONFIG_NODE_BATTLE_PARTY_SIZE = "battle_party_size";

    protected static string CONFIG_NODE_RAIDER_PARTY_SIZE_MIN = "raider_party_size_min";

    protected static string CONFIG_NODE_RAIDER_PARTY_SIZE_MAX = "raider_party_size_max";

    protected static string LOOT_LIST_XML_PATH = "LootList/Loot";
    protected static string XML_ATTRIBUTE_ID = "id";

    public int RaidingPartyCount => MobileParty.All.Where(x => x.IsRaidingParty() && x.HomeSettlement == Settlement).Count(); //TODO: Sly : that's a lot of parties being checked - Implement OnPartyCreated / OnPartyDestoyed maybe?

    public abstract string BattleSceneName { get; }

    public abstract string RaiderTemplate { get; }

    public abstract string RaiderPartyName { get; }

    public abstract int BattlePartySize { get; }

    public abstract int RaiderPartyMinSize { get; }

    public abstract int RaiderPartyMaxSize { get; }

    public bool IsBattleUnderway { get; set; }
    public abstract List<string> RewardItemIds { get; }
    public abstract MobileParty SpawnNewParty(Settlement initialTarget);
}