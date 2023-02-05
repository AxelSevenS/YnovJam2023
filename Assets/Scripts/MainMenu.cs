using UnityEngine;
using UnityEngine.UI;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using TMPro;
using System.Net;
using System.Net.Sockets;

public class MainMenu : MonoBehaviour {

    [SerializeField] private GameObject panel;

    [SerializeField] private Button hostButton;
	[SerializeField] private TextMeshProUGUI ipAddressDisplay;
    
    [SerializeField] private Button joinButton;
	[SerializeField] private TMP_InputField ipInput;

	[SerializeField] private string ipAddress;
	[SerializeField] private UnityTransport transport;

	void Start() {
		ipAddress = "0.0.0.0";

        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(StartClient);

		SetIpAddress();
        GetLocalIPAddress();
	}

	// To Host a game
	public void StartHost() {
		NetworkManager.Singleton.StartHost();
		GetLocalIPAddress();
        panel.SetActive(false);
	}

	// To Join a game
	public void StartClient() {
		ipAddress = ipInput.text;
		SetIpAddress();
		NetworkManager.Singleton.StartClient();
        panel.SetActive(false);
	}

	public string GetLocalIPAddress() {
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ip in host.AddressList) {
			if (ip.AddressFamily == AddressFamily.InterNetwork) {
				ipAddressDisplay.text = ip.ToString();
				ipAddress = ip.ToString();
				return ip.ToString();
			}
		}
		throw new System.Exception("No network adapters with an IPv4 address in the system!");
	}

	public void SetIpAddress() {
		transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
		transport.ConnectionData.Address = ipAddress;
	}
  
}