using GameLib.Random;
using UnityEngine;

public class FaceShaderController : MonoBehaviour
{
    public enum State
    {
        OpenedEyes,
        ClosedEyes
    }

    public int EmojiAmountPerRaw;
    public int EmojiRawsCount;
    public Range BlinkDelay;
    public float PerBlinkDuration;
    public State CurrentState { get; private set; }

    private float _curStateRemainingDuration;
    private int _currentEmojiIndex;
    private Vector2 _currentEmojiOffset;
    private Vector2 _currentBlinkOffset;

    public Renderer Renderer;

    void Update()
    {
        _curStateRemainingDuration -= Time.deltaTime;
        if (CurrentState == State.OpenedEyes)
        {
            if (_curStateRemainingDuration < 0f)
            {
                // goto closed eyes state
                CurrentState = State.ClosedEyes;
                _curStateRemainingDuration = PerBlinkDuration;
                SetBlinkOffset();
                SetFrame();
            }
        }
        else if (CurrentState == State.ClosedEyes)
        {
            if (_curStateRemainingDuration < 0f)
            {
                // goto opened eyes state
                CurrentState = State.OpenedEyes;
                _curStateRemainingDuration = Random.Range(BlinkDelay.From, BlinkDelay.To);
                SetBlinkOffset();
                SetFrame();
            }
        }
    }

    public void SetEmoji(int index)
    {
        SetEmojiOffset(index);
        SetFrame();
    }

    private void SetEmojiOffset(int index)
    {
        _currentEmojiIndex = index;
        int gridCoordX = index % EmojiAmountPerRaw;
        int gridCoordY = index / EmojiAmountPerRaw;

        float x = gridCoordX * 1f / EmojiAmountPerRaw;
        float y = 1f - gridCoordY * 1f / EmojiRawsCount;

        _currentEmojiOffset = new Vector2(x, y);
    }

    private void SetBlinkOffset()
    {
        if (CurrentState == State.OpenedEyes)
            _currentBlinkOffset = Vector2.zero;
        else
            _currentBlinkOffset = new Vector2(1f / EmojiAmountPerRaw * 0.5f, 0f);
    }

    public void SetFrame()
    {
        Renderer.sharedMaterial.mainTextureOffset = _currentEmojiOffset + _currentBlinkOffset;
    }

    [ContextMenu("DbgSetEmoji0")]
    void TestSetEmoji0()
    {
        SetEmoji(0);
        SetFrame();
    }

    [ContextMenu("DbgSetEmoji1")]
    void TestSetEmoji1()
    {
        SetEmoji(1);
        SetFrame();
    }
}
