using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton

    public static EquipmentManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public Equipment[] defaultItems;

    // targetMesh -> player body
    public SkinnedMeshRenderer targetMesh;
    Equipment[] currentEquipment;
    SkinnedMeshRenderer[] currentMeshes;

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    Inventory inventory;

    void Start()
    {
        inventory = Inventory.Instance;

        // todo: initial equipment array length

        //System.Enum.GetNames(typeof(EquipmentSlot)).Length is use to get enum length
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];
        currentMeshes = new SkinnedMeshRenderer[numSlots];

        EquipDefaultItems();
    }

    // equip a new item
    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.equipSlot;
        
        Equipment oldItem = UnEquip(slotIndex);

        // change status when equip item
        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        // update body size make it don't through the equipment
        SetEquipmentBlendShapes(newItem, 100);

        // insert the item into the slot
        currentEquipment[slotIndex] = newItem;

        // use Instantiate to copy the prefab object, not use new object to create game object
        SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.mesh);
        newMesh.transform.parent = targetMesh.transform;

        // make equipment will follow the player body
        newMesh.bones = targetMesh.bones;
        newMesh.rootBone = targetMesh.rootBone;
        currentMeshes[slotIndex] = newMesh;
    }

    // unequip an item with a particular index
    public Equipment UnEquip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            if (currentMeshes[slotIndex] != null)
            {
                Destroy(currentMeshes[slotIndex].gameObject);
            }
            Equipment oldItem = currentEquipment[slotIndex];
            // set body size to original size
            SetEquipmentBlendShapes(oldItem, 0);

            // put equipped item to inventory
            inventory.Add(oldItem);

            currentEquipment[slotIndex] = null;

            // change status when unequip item
            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }
            return oldItem;
        }
        return null;
    }

    public void UnEquipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            UnEquip(i);
        }
        // equip default shoes, shirt and so on
        EquipDefaultItems();
    }

    void SetEquipmentBlendShapes(Equipment item, int weight)
    {
        foreach(EquipmentMeshRegion blendShape in item.coveredMeshRegion)
        {
            // if setBlendShapeWeight not work but don't show any error
            // 1. open player.blend and select body in collection1
            // 2. find shape keys and uncheck relative option
            targetMesh.SetBlendShapeWeight((int)blendShape, weight);
        }
    }
    void EquipDefaultItems()
    {
        foreach(Equipment item in defaultItems)
        {
            Equip(item);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) 
        {
            UnEquipAll();
        }
    }
}
