using ScriptableObjectArchitecture;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PinCollection.asset",
    menuName = SOArchitecture_Utility.COLLECTION_SUBMENU + "Pin",
    order = SOArchitecture_Utility.ASSET_MENU_ORDER_COLLECTIONS + 0)]
public class PinCollection : Collection<Pin> { }