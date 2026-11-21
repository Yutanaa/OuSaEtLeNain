using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class QTEsys : MonoBehaviour
{
    public GameObject _RightKeyUi;
    public GameObject _LeftKeyUi;
    public GameObject _UpKeyUi;
    public GameObject _DownKeyUi;
    public int _qteGen;
    public int _waitingForKey;
    public int _correctKey;
    public int _countingDown;
    PlayerInput _playerInput;
    InputAction _leftKey;
    InputAction _rightKey;
    InputAction _upKey;
    InputAction _downKey;

    private void Start()
    {
        _leftKey = InputSystem.actions.FindAction("Left");
        _rightKey = InputSystem.actions.FindAction("Right");
        _upKey = InputSystem.actions.FindAction("Up");
        _downKey = InputSystem.actions.FindAction("Down");
    }

    private void Update()
    {
        if (_waitingForKey == 0)
        {
            _qteGen = Random.Range(1, 5);
            _countingDown = 1;
            if (_qteGen == 1)
            {
                _waitingForKey = 1;
                _LeftKeyUi.SetActive(true);
                StartCoroutine(CountingDown(_LeftKeyUi));
            }

            if (_qteGen == 2)
            {
                _waitingForKey = 1;
                _RightKeyUi.SetActive(true);
                StartCoroutine(CountingDown(_RightKeyUi));
            }

            if (_qteGen == 3)
            {
                _waitingForKey = 1;
                _UpKeyUi.SetActive(true);
                StartCoroutine(CountingDown(_UpKeyUi));
            }

            if (_qteGen == 4)
            {
                _waitingForKey = 1;
                _DownKeyUi.SetActive(true);
                StartCoroutine(CountingDown(_DownKeyUi));
            }
        }

        if (_qteGen == 1)
        {
            if (_leftKey.IsPressed())
            {
                _correctKey = 1;
                StartCoroutine(PressingKey(_LeftKeyUi));
            }
            else
            {
                _correctKey = 2;
                StartCoroutine(PressingKey(_LeftKeyUi));
            }
        }

        if (_qteGen == 2)
        {
            if (_rightKey.IsPressed())
            {
                _correctKey = 1;
                StartCoroutine(PressingKey(_RightKeyUi));
            }
            else
            {
                _correctKey = 2;
                StartCoroutine(PressingKey(_RightKeyUi));
            }
        }
        if (_qteGen == 3)
        {
            if (_upKey.IsPressed())
            {
                _correctKey = 1;
                StartCoroutine(PressingKey(_UpKeyUi));
            }
            else
            {
                _correctKey = 2;
                StartCoroutine(PressingKey(_UpKeyUi));
            }
        }
        if (_qteGen == 4)
        {
            if (_downKey.IsPressed())
            {
                _correctKey = 1;
                StartCoroutine(PressingKey(_DownKeyUi));
            }
            else
            {
                _correctKey = 2;
                StartCoroutine(PressingKey(_DownKeyUi));
            }
        }

        IEnumerator PressingKey(GameObject keyUi)
        {
            _qteGen = 10;

            if (_correctKey == 1)
            {
                _countingDown = 2;
                yield return new WaitForSeconds(1);
                _correctKey = 0;
                keyUi.SetActive(false);
                yield return new WaitForSeconds(1);
                _waitingForKey = 0;
                _countingDown = 1;

            }
            if (_correctKey == 2)
            {
                _countingDown = 2;
                yield return new WaitForSeconds(1);
                _correctKey = 0;
                keyUi.SetActive(false);
                yield return new WaitForSeconds(1);
                _waitingForKey = 0;
                _countingDown = 1;

            }
        }

        IEnumerator CountingDown(GameObject keyUi)
        {
            yield return new WaitForSeconds(2);
            if (_countingDown == 1)
            {
                _qteGen = 10;
                _countingDown = 2;
                yield return new WaitForSeconds(1f);
                _correctKey = 0;
                keyUi.SetActive(false);
                yield return new WaitForSeconds(1f);
                _waitingForKey = 0;
                _countingDown = 1;
            }
            
        }
    }
}
