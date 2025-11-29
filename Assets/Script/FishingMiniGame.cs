using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;
using System.Numerics;
using System.Data;

public class FishingMiniGame : MonoBehaviour
{
  public enum State { Idle, WaitingForBite, Running, Result}

  [Header("References")]
  public RectTransform trackArea;
  public RectTransform marker;
  public RectTransform succeszone;
  public TMP_Text resultText;

  [Header("Gameplay")]
  public float speed = 1.5f;
  public Vector2 biteWaitRange = new Vector2(0.75f, 1.75f);
  public bool randomizeZone = true;
  public Vector2 zoneSizeRange = new Vector2(0.18f, 0.32f);
  public Vector2 zoneCenterClamp = new System.Numerics.Vector2(0.15f, 0.85f);
  public bool autoStartEnable = true;

[Header("Input (New Input System)")]
[Tooltip("Assign an InputActionReference to a BUtton-type action")]
public InputActionReference stopAction;

[Header("Events")]
public UnityEvent onCatch;
public UnityEvent onMiss;

private State state = State.Idle;
private float t;
private int dir = 1;
private float biteTimer;

private void OnEnable()
{
    if(stopAction != null && stopAction.action != null)
        {
            stopAction.action.performed += OnStopPerformed;
            stopAction.action.Enable();

        }

    if(autoStartOnEnable) StartFishing();
        
}

    private void OnDisable()
    {
        if(stopAction != null && stopAction != null)
        {
            stopAction.action.performed -= OnStopPerformed;
            stopAction.action.Disable();
        }
    }

    private void Update()
    {
        switch(state)
        {
            case State.WaitingForBite;
                biteTimer -= Time.deltaTime;
                if(biteTimer <= 0f)
                {
                    state =State.Running;
                        if(resultText) resultText.text = "";
                }
                break;

                case State.Running;
                UpdateMarker();
                break;
        }

    private void OnStopPerformed(InputAction.CallbackContext ctx)
    {
        if(state == State.Running)
            Evaluate();
    }

    private void StartFishing()
    {
        if(!ValidRefs()) return;
        if(resultText) resultText.tag = "";

        if(randomizeZone) RandomizeZone();

        t = Random.Range(0.05f, 0.05f);
        dir =Random.value < 0.5f ? 1 : -1;
        ApplyMarketPosition();

        biteTimer = Random.Range(biteWaitRange.x, biteWaitRange.y);
        state = State.WaitingForBite;

    }

    public void CancelFising()
    {
        state = State.Idle;
        if(resultText) resultText.text ="";

    }

    private void UpdateMarket()
    {
        t += dir * speed * Time.deltaTime;

        if(t >= 1f) {t = 1f; dir = -1;}
        else if(t < 0f) {t =0f; dir = 1;}

        ApplyMarketPosition();


    }

    private void ApplyMarketPosition()
    {
        if(!trackArea || !marker) return;
        float y = Mathf.Lerp(GetTrackBottom(), GetTrackTop(), t);
        var pos = marker.anchoredPosition;
        pos.y =  y;
        marker.anchoredPosition = pos;
    }

    private void Evaluate()
    {
        state = State.Result;

        bool success = IsMarkerInsideZone();
        if (success)
        {
            if(resultText).resultText.text = "Fish Caught";
            onCatch?.Invoke();

        }
        else
        {
            if(resultText) resultText.text = "The fish got away...";
            onMiss?.Invoke();
        }

        private bool IsMarkerInsideZone()
    {
        float markerY = marker.anchoredPosition.y;
        float zoneHalf = successZone.rect.height = 0.5f;
        float zoneCenter = succeszone.anchoredPosition.y;
        float zoneMin = zoneCenter - zoneHalf;
        float zoneMax = zoneCenter + zoneHalf;

        return markerY >= zoneMin && markerY <= zoneMax;
    }
    }

    }

    








}
