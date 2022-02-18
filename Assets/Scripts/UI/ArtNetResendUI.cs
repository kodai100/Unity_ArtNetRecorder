using System.Collections;
using System.Collections.Generic;
using System.Net;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ArtNetResendUI : MonoBehaviour
{

    [SerializeField] private Toggle enableToggle;
    [SerializeField] private InputField ipInputField;
    [SerializeField] private InputField portInputField;

    public bool IsEnabled => isValidated && enableToggle.isOn;
    public int Port => port;
    public IPAddress IPAddress => ipAddress;

    private bool isValidated;

    private int port;
    private IPAddress ipAddress;

    private void Start()
    {
        ipInputField.OnValueChangedAsObservable().Subscribe(t =>
        {
            if (IPAddress.TryParse(t, out var address))
            {
                isValidated = true;
                ipAddress = address;
                ipInputField.image.color = Color.cyan;
            }
            else
            {
                isValidated = false;
                ipInputField.image.color = Color.red;
            }
        }).AddTo(this);

        portInputField.OnValueChangedAsObservable().Subscribe(t =>
        {
            if (int.TryParse(t, out var value))
            {
                isValidated = true;
                port = value;
                portInputField.image.color = Color.cyan;
            }
            else
            {
                isValidated = false;
                portInputField.image.color = Color.red;
            }
        }).AddTo(this);
    }
    

}
