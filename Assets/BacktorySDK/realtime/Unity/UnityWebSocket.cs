using Assets.BacktorySDK.auth;
using Sdk.Core;
using Sdk.Unity.WS;
using System;
using System.Collections.Generic;

namespace Sdk.Unity
{
	public abstract class UnityWebSocket : BacktoryWebSocketAbstractionLayer {
		// TODO: 7/19/16 AD IMPORTANT: Remove
		public static string token;
		
		private BacktoryStompWebSocket webSocketClient;

		public abstract Dictionary<string, string> getExtraHeaders();

		public UnityWebSocket(string url, WebSocketListener webSocketListener, string xBacktoryInstanceId) : base(url, webSocketListener, xBacktoryInstanceId) {
		}
		
		override
		public void Connect(string matchId) {
			
//			URI uri;
//			try {
//				uri = new URI(url);
//			} catch (URISyntaxException e) {
//				e.printStackTrace();
//				// TODO: 7/13/16 AD What to do?
//				return;
//			}
			Dictionary<string, string> extraHeaders = new Dictionary<string, string>();
			// TODO: uncomment
//			extraHeaders.Add("user-agent", System.getProperty("http.agent"));
			// TODO: 7/13/16 AD WebSocket should gather token in true manner
			//        extraHeaders.put("Authorization", "Bearer " + BacktoryAuth.getAccessToken());
			extraHeaders.Add("Authorization-Bearer", BacktoryUser.GetAccessToken());
			extraHeaders.Add("X-Backtory-Connectivity-Id", X_BACKTORY_CONNECTIVITY_ID);
			if (matchId != null) {
				extraHeaders.Add("X-Backtory-Realtime-Challenge-Id", matchId);
			}

			InnerStompWebSocketEventHandler innerEventHandler = new InnerStompWebSocketEventHandler (this);
			webSocketClient = new InnerBacktoryStompWebSocket (this, url, extraHeaders, innerEventHandler);

			webSocketClient.Connect();
		}

		override
		public void Disconnect() {
			webSocketClient.Disconnect();
		}
		
		override
		public void Send(string destination, string body, Dictionary<string, string> extraHeader) {
			Dictionary<string, string> xBacktoryHeader = new Dictionary<string, string>();
			xBacktoryHeader.Add("X-Backtory-Connectivity-Id", X_BACKTORY_CONNECTIVITY_ID);
			if (extraHeader != null) {
				foreach (KeyValuePair<string, string> header in extraHeader) {
					if (header.Key != null && header.Value != null)
						xBacktoryHeader.Add(header.Key, header.Value);
				}
			}
			webSocketClient.send(destination, xBacktoryHeader, body);
		}
		
		override
		public bool IsAlive() {
			// TODO correct it
//			return webSocketClient.IsAlive();
			return true;
		}
		
		internal class InnerBacktoryStompWebSocket : BacktoryStompWebSocket {

			private UnityWebSocket outClass;

			internal InnerBacktoryStompWebSocket(UnityWebSocket unityWebSocket, 
												 string url, 
												 Dictionary<string, 
												 string> extraHeaders, 
												 StompWebSocketEventHandler eventHandlerl) : base(url, extraHeaders, eventHandlerl) {
				outClass = unityWebSocket;
			}

			override
			public Dictionary<string, string> getExtraHeaders() {
				return outClass.getExtraHeaders();
			}

		}

		internal class InnerStompWebSocketEventHandler : StompWebSocketEventHandler {

			UnityWebSocket outClass;

			internal InnerStompWebSocketEventHandler(UnityWebSocket unityWebSocket) {
				outClass = unityWebSocket;
			}

			public void OnMessage(string message) {
				outClass.webSocketListener.OnMessage(message);
			}

			public void OnConnected(string username, string userId) {
				outClass.setUserInformation(username, userId);
				outClass.webSocketListener.OnConnect();
			}

			public void OnDisconnected() {
				outClass.webSocketListener.OnDisconnect ();
			}

			public void OnError(Exception exception) {
				outClass.webSocketListener.OnError(exception);
			}

		}

	}
}
