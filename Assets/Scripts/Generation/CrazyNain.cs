

public abstract class CrazyNain : Interactible
{
    public bool _isDown = false;

    public void Hit()
    {
        if (!_isDown)
        {
            _isDown = true;
            PlayDownAnimation();
        }
    }

    public abstract void PlayDownAnimation();


}