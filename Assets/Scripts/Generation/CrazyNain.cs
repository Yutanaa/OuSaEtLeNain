

public abstract class CrazyNain : NainBase
{
    public bool _isDown = false;

    public virtual void Hit()
    {
        if (!_isDown)
        {
            _isDown = true;
            PlayDownAnimation();
        }
    }

    public abstract void PlayDownAnimation();


}