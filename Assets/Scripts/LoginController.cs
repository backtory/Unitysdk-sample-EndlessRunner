using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Assets.BacktorySDK.auth;
using Assets.BacktorySDK.core;

public class LoginController : MonoBehaviour {

	public InputField loginUsername;
	public InputField loginPassword;

	public InputField registerFirstname;
	public InputField registerLastname;
	public InputField registerUsername;
	public InputField registerPassword;
	public InputField registerEmail;

	public void OnLoginButtonClicked() {
		BacktoryUser.LoginInBackground (loginUsername.text, loginPassword.text, loginResponse => {
			if (loginResponse.Successful) {
				Debug.Log ("Welcome, " + loginUsername.text + "!");
				SceneManager.LoadScene ("Main");
			} else if (loginResponse.Code == (int)BacktoryHttpStatusCode.Unauthorized)  {
				Debug.Log("Either username or password is wrong");
			} else {
				Debug.Log ("Strange! Login Failed! code: " + loginResponse.Code);
			}
		});
	}

	public void OnRegisterButtonClicked() {
		BacktoryUser newUser = new BacktoryUser.Builder ()
			.SetFirstName (registerFirstname.text)
			.SetLastName (registerLastname.text)
			.SetUsername (registerUsername.text)
			.SetPassword (registerPassword.text)
			.SetEmail (registerEmail.text)
			.build ();

		newUser.RegisterInBackground (userResponse => {
			if (userResponse.Successful) {
				Debug.Log ("Register success: new username is " + userResponse.Body.Username);
			} else if (userResponse.Code == (int) BacktoryHttpStatusCode.Conflict) {
				Debug.Log("Bad username: a user with this username already exist");
			} else {
				Debug.Log("Strange! Registeration failed! code: " + userResponse.Code);
			}
		});
	}
}
