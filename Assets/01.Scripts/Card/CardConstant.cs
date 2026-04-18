public enum CardType
{
    None,
    Playing,
    Joker,
    Taro,
    Planet
}

public enum TaroType
{
    None,
    Enforce,
    Change,
    Remove
}

public enum HandRanking
{
    None,
    HighCard,
    OnePair,
    TwoPair,
    Triple,
    FourCard,
    Straight,
    Flush,
    FullHouse,
    StraightFlush,
}

public enum Suit
{
    Spade,
    Diamond,
    Heart,
    Club,
    None
}

public enum JokerEffectType
{
    None,
    AddTwicePictureRank,
    AddMultCaseRanking,
    UseUnderThree,
    SpadeBonus,
    DiamondBonus,
    HeartBonus,
    ClubBonus,
    BuildMult,
}