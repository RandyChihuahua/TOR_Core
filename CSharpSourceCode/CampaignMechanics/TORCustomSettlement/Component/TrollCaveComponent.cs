using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using TOR_Core.CampaignMechanics.RaidingParties;
using TOR_Core.Utilities;

namespace TOR_Core.CampaignMechanics.TORCustomSettlement.Component;
public class TrollCaveComponent : BaseRaiderSpawnerComponent
{
    private int _raiderPartyMinSize;
    private int _raiderPartyMaxSize;
    private int _battlePartySize;
    private string _battleSceneName;
    private string _raiderTemplate;
    private string _raiderPartyName;
    private List<string> _itemRewardIds = [];

    public override int BattlePartySize => _battlePartySize;

    public override int RaiderPartyMinSize => _raiderPartyMinSize;

    public override int RaiderPartyMaxSize => _raiderPartyMaxSize;
    public override string BattleSceneName => _battleSceneName;
    public override string RaiderTemplate => _raiderTemplate;
    public override string RaiderPartyName => _raiderPartyName;


    public override List<string> RewardItemIds => _itemRewardIds; //TODO: Let's discuss this with DEV team.. Gandalf would be disappointed otherwise. Troll hoards are where he found Glamdring and Sting.
    public override IFaction MapFaction => Settlement.Owner.Clan;

    public override void Deserialize(MBObjectManager objectManager, XmlNode node)
    {
        base.Deserialize(objectManager, node);

        if (string.IsNullOrEmpty(node.Attributes[CONFIG_NODE_BATTLE_SCENE].Value)) 
        {
            throw new MBMisuseException($"{CONFIG_NODE_BATTLE_SCENE} missing from {this.GetType()}");
        }
        _battleSceneName = node.Attributes[CONFIG_NODE_BATTLE_SCENE].Value;

        if (string.IsNullOrEmpty(node.Attributes[CONFIG_NODE_RAIDER_TEMPLATE].Value))
        {
            throw new MBMisuseException($"{CONFIG_NODE_RAIDER_TEMPLATE} missing from {this.GetType()}");
        }
        _raiderTemplate = node.Attributes[CONFIG_NODE_RAIDER_TEMPLATE].Value;

        if (string.IsNullOrEmpty(node.Attributes[CONFIG_NODE_RAIDER_PARTY_NAME].Value))
        {
            throw new MBMisuseException($"{CONFIG_NODE_RAIDER_PARTY_NAME} missing from {this.GetType()}");
        }
        _raiderPartyName = node.Attributes[CONFIG_NODE_RAIDER_PARTY_NAME].Value;

        if (!int.TryParse(node.Attributes[CONFIG_NODE_BATTLE_PARTY_SIZE]?.Value, out var battlePartySize))
        {
            throw new MBMisuseException($"{CONFIG_NODE_BATTLE_PARTY_SIZE} missing from {this.GetType()}");
        }
        _battlePartySize = battlePartySize;

        if (!int.TryParse(node.Attributes[CONFIG_NODE_RAIDER_PARTY_SIZE_MIN]?.Value, out var raiderPartyMinSize))
        {
            throw new MBMisuseException($"{CONFIG_NODE_RAIDER_PARTY_SIZE_MIN} missing from {this.GetType()}");
        }
        _raiderPartyMinSize = raiderPartyMinSize;

        if (!int.TryParse(node.Attributes[CONFIG_NODE_RAIDER_PARTY_SIZE_MAX]?.Value, out var raiderPartyMaxSize))
        {
            throw new MBMisuseException($"{CONFIG_NODE_RAIDER_PARTY_SIZE_MAX} missing from {this.GetType()}");
        }
        _raiderPartyMaxSize = raiderPartyMaxSize;

        PopulateLoot(node);

    }

    private void PopulateLoot(XmlNode node)
    {
        var lootNodes = node.SelectNodes(LOOT_LIST_XML_PATH);
        if (lootNodes?.Count == 0)
        {
            return;
        }
        foreach (XmlNode loot in lootNodes)
        {
            var lootItem = loot.Attributes[XML_ATTRIBUTE_ID].Value;
            if (!string.IsNullOrEmpty(lootItem)) { _itemRewardIds.Add(lootItem); }
            ;
        }
    }

    public override MobileParty SpawnNewParty(Settlement initialTarget)
    {
        PartyTemplateObject template = MBObjectManager.Instance.GetObject<PartyTemplateObject>(RaiderTemplate);
        var find = TORCommon.FindSettlementsAroundPosition(Settlement.Position.ToVec2(), 60, x => !x.IsRaided && !x.IsUnderRaid && x.IsVillage).GetRandomElementInefficiently();
        var trollRaidingParty = RaidingPartyComponent.CreateRaidingParty("troll_clan_1_party_" + RaidingPartyCount + 1, 
            Settlement, CONFIG_NODE_RAIDER_PARTY_NAME, template, MBRandom.RandomInt(RaiderPartyMinSize, RaiderPartyMaxSize));
        if (find != null)
        {
            SetPartyAiAction.GetActionForRaidingSettlement(trollRaidingParty, initialTarget ?? find, MobileParty.NavigationType.Default, false);
            ((RaidingPartyComponent)trollRaidingParty.PartyComponent).Target = initialTarget ?? find;
        }
        else
        {
            ((RaidingPartyComponent)trollRaidingParty.PartyComponent).Target = null;
        }

        return trollRaidingParty;
    }
}