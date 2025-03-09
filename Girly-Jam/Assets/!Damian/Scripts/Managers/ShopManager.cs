using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public List<Furniture> furnitureItems;

    public GameObject spawnZone;

    private Dictionary<Button, float> lastClickTimes = new Dictionary<Button, float>();

    private const float doubleClickThreshold = 0.3f;

    void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
    }

    void OnButtonClicked(Button clickedButton)
    {
        float currentTime = Time.time;

        if (lastClickTimes.TryGetValue(clickedButton, out float lastTime) && 
            currentTime - lastTime < doubleClickThreshold)
        {
            Image image = clickedButton.GetComponentInChildren<Image>();
            if (image == null || image.sprite == null)
            {
                Debug.LogError("Button has no Image or sprite assigned.");
                return;
            }

            string spriteName = image.sprite.name;
            int dollarIndex = spriteName.IndexOf("$");
            if (dollarIndex == -1)
            {
                Debug.LogError("Invalid sprite name: no '$' found in " + spriteName);
                return;
            }
            string furnitureName = spriteName.Substring(0, dollarIndex).Trim();

            Furniture selectedFurniture = furnitureItems.Find(f => f.itemName == furnitureName);
            if (selectedFurniture == null)
            {
                Debug.LogError("No Furniture SO found for name: " + furnitureName);
                return;
            }

            if (spawnZone == null)
            {
                Debug.LogError("Spawn zone not assigned in ShopManager.");
                return;
            }
            BoxCollider collider = spawnZone.GetComponent<BoxCollider>();
            if (collider == null)
            {
                Debug.LogError("Spawn zone has no BoxCollider.");
                return;
            }

            Vector3 min = collider.bounds.min;
            Vector3 max = collider.bounds.max;
            Vector3 randomPos = new Vector3(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y),
                Random.Range(min.z, max.z)
            );

            GameObject spawnedModel = Instantiate(
                selectedFurniture.model, 
                randomPos, 
                Quaternion.Euler(-90, -180, 0)
            );

            Rigidbody rb = spawnedModel.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = spawnedModel.AddComponent<Rigidbody>();
            }

            if (spawnedModel.GetComponent<Collider>() == null)
            {
                spawnedModel.AddComponent<BoxCollider>();
            }

            gameObject.SetActive(false);

            lastClickTimes.Remove(clickedButton);
        }
        else
        {
            lastClickTimes[clickedButton] = currentTime;
        }
    }
}