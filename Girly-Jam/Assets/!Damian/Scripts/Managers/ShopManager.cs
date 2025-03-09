using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
#if UNITY_EDITOR
using TMPro;
#endif

public class ShopManager : MonoBehaviour
{
    public List<Furniture> furnitureItems;
    public GameObject spawnZone;
    public GameObject popupPanel;
    private RectTransform popupRectTransform;
    private Dictionary<Button, float> lastClickTimes = new Dictionary<Button, float>();
    private const float doubleClickThreshold = 0.3f;
    public GameObject previewPanel;
    public Image previewImage;
#if UNITY_EDITOR
    public TextMeshProUGUI previewText;
    public TextMeshProUGUI costText;  
#else
    public Text previewText; 
    public Text costText;   
#endif

    private RectTransform previewRectTransform;

    void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((data) => { OnPointerEnter(button); });
            trigger.triggers.Add(entryEnter);
            EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener((data) => { OnPointerExit(); });
            trigger.triggers.Add(entryExit);
        }

        if (popupPanel != null)
        {
            popupRectTransform = popupPanel.GetComponent<RectTransform>();
            popupPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("PopupPanel not assigned in ShopManager!");
        }

        if (previewPanel != null)
        {
            previewRectTransform = previewPanel.GetComponent<RectTransform>();
            previewPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("PreviewPanel not assigned in ShopManager!");
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

            int playerPoints = PointManager.Instance.getTotalPoints();
            if (playerPoints >= selectedFurniture.cost)
            {
                PointManager.Instance.decreasePoints(selectedFurniture.cost);

                if (spawnZone == null || spawnZone.GetComponent<BoxCollider>() == null)
                {
                    Debug.LogError("Spawn zone not assigned or missing BoxCollider.");
                    return;
                }

                BoxCollider collider = spawnZone.GetComponent<BoxCollider>();
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
                if (rb == null) rb = spawnedModel.AddComponent<Rigidbody>();
                if (spawnedModel.GetComponent<Collider>() == null)
                {
                    spawnedModel.AddComponent<BoxCollider>();
                }

                spawnedModel.AddComponent<ObjectController>();

                gameObject.SetActive(false);
                lastClickTimes.Remove(clickedButton);
            }
            else
            {
                ShowPopupAtClickPosition();
            }
        }
        else
        {
            lastClickTimes[clickedButton] = currentTime;
        }
    }

    private void ShowPopupAtClickPosition()
    {
        if (popupPanel == null || popupRectTransform == null) return;

        Canvas canvas = popupPanel.GetComponentInParent<Canvas>();
        if (canvas == null) return;

        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;
        Vector2 mousePosition = Input.mousePosition;
        RectTransform parentRect = popupRectTransform.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            mousePosition,
            cam,
            out Vector2 localPoint
        );

        popupRectTransform.localPosition = localPoint;
        popupPanel.SetActive(true);
        Invoke("HidePopup", 2f);
    }

    private void HidePopup()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }

    private void OnPointerEnter(Button button)
    {
        if (previewPanel == null || previewImage == null || previewText == null || costText == null || !gameObject.activeInHierarchy)
            return;

        Image buttonImage = button.GetComponentInChildren<Image>();
        if (buttonImage == null || buttonImage.sprite == null) return;

        string spriteName = buttonImage.sprite.name;
        int dollarIndex = spriteName.IndexOf("$");
        if (dollarIndex == -1) return;
        string itemName = spriteName.Substring(0, dollarIndex).Trim();

        previewImage.sprite = buttonImage.sprite;

        Furniture selectedFurniture = furnitureItems.Find(f => f.itemName == itemName);
        if (selectedFurniture != null)
        {
            previewText.text = itemName;
            costText.text = $"Cost: {selectedFurniture.cost} points";
        }
        else
        {
            previewText.text = itemName;
            costText.text = "Cost: N/A";
            Debug.LogWarning($"No Furniture SO found for name: {itemName}");
        }

        PositionPreview();
        previewPanel.SetActive(true);
    }

    private void OnPointerExit()
    {
        if (previewPanel != null)
        {
            previewPanel.SetActive(false);
        }
    }

    private void PositionPreview()
    {
        if (previewPanel == null || previewRectTransform == null) return;

        Vector2 mousePosition = Input.mousePosition;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        Vector2 previewSize = new Vector2(
            previewRectTransform.rect.width * previewRectTransform.localScale.x,
            previewRectTransform.rect.height * previewRectTransform.localScale.y
        );

        float offset = 20f;

        Vector2 rightPosition = new Vector2(mousePosition.x + previewSize.x + offset, mousePosition.y + offset);
        Vector2 leftPosition = new Vector2(mousePosition.x - (2 * previewSize.x + offset), mousePosition.y + offset);
        Vector2 abovePosition = new Vector2(mousePosition.x + offset, mousePosition.y + previewSize.y + offset);
        Vector2 belowPosition = new Vector2(mousePosition.x + offset, mousePosition.y - (2 * previewSize.y + offset));

        bool canPlaceRight = (rightPosition.x + previewSize.x <= screenWidth) && 
                            (rightPosition.y + previewSize.y <= screenHeight) && 
                            (rightPosition.y >= 0);
        bool canPlaceLeft = (leftPosition.x >= 0) && 
                           (leftPosition.y + previewSize.y <= screenHeight) && 
                           (leftPosition.y >= 0);
        bool canPlaceAbove = (abovePosition.y + previewSize.y <= screenHeight) && 
                            (abovePosition.x + previewSize.x <= screenWidth) && 
                            (abovePosition.x >= 0);
        bool canPlaceBelow = (belowPosition.y >= 0) && 
                            (belowPosition.x + previewSize.x <= screenWidth) && 
                            (belowPosition.x >= 0);

        Vector2 previewPosition = Vector2.zero;
        bool positionFound = false;

        if (canPlaceRight && !IsOverlapping(rightPosition, mousePosition, previewSize))
        {
            previewPosition = rightPosition;
            positionFound = true;
        }
        else if (canPlaceLeft && !IsOverlapping(leftPosition, mousePosition, previewSize))
        {
            previewPosition = leftPosition;
            positionFound = true;
        }
        else if (canPlaceBelow && !IsOverlapping(belowPosition, mousePosition, previewSize))
        {
            previewPosition = belowPosition;
            positionFound = true;
        }
        else if (canPlaceAbove && !IsOverlapping(abovePosition, mousePosition, previewSize))
        {
            previewPosition = abovePosition;
            positionFound = true;
        }

        if (!positionFound)
        {
            Vector2[] safePositions = new Vector2[]
            {
                new Vector2((screenWidth - previewSize.x) / 2, (screenHeight - previewSize.y) / 2), // Center
                new Vector2(screenWidth - previewSize.x - offset, screenHeight - previewSize.y - offset), // Bottom-right
                new Vector2(offset, offset), // Top-left
                new Vector2(screenWidth - previewSize.x - offset, offset), // Top-right
                new Vector2(offset, screenHeight - previewSize.y - offset) // Bottom-left
            };

            float maxDistance = -1f;
            Vector2 bestPosition = Vector2.zero;

            foreach (Vector2 pos in safePositions)
            {
                if (pos.x >= 0 && pos.x + previewSize.x <= screenWidth &&
                    pos.y >= 0 && pos.y + previewSize.y <= screenHeight &&
                    !IsOverlapping(pos, mousePosition, previewSize))
                {
                    float distance = Vector2.Distance(pos, mousePosition);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        bestPosition = pos;
                    }
                }
            }

            previewPosition = bestPosition != Vector2.zero ? bestPosition : safePositions[0]; // Default to center if none fit
        }

        Canvas canvas = previewPanel.GetComponentInParent<Canvas>();
        if (canvas == null) return;

        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            previewPosition,
            cam,
            out Vector2 localPoint
        );

        previewRectTransform.localPosition = localPoint;

        bool IsOverlapping(Vector2 panelPosition, Vector2 mousePos, Vector2 panelSize)
        {
            float panelRight = panelPosition.x + panelSize.x;
            float panelTop = panelPosition.y + panelSize.y;
            return mousePos.x >= panelPosition.x && mousePos.x <= panelRight &&
                   mousePos.y >= panelPosition.y && mousePos.y <= panelTop;
        }
    }
}