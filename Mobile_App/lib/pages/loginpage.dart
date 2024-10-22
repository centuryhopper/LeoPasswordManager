import 'package:flutter/material.dart';
import 'package:mobile_app/Services/AuthService.dart';
import 'package:mobile_app/main.dart';

class LoginPage extends StatefulWidget {
  static const String routeID = "/login";
  const LoginPage({
    super.key,
    String appName = "",
    AppHome? appHome,
  });

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> with WidgetsBindingObserver {
  AuthService authService = AuthService();

  bool rememberMe = false;
  bool obscureText = true;
  final TextEditingController? emailController = TextEditingController();
  final TextEditingController? passwordController = TextEditingController();

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this); // Add observer

    // for testing purposes
    // AuthService.clearLogin();

    var waitForToken = AuthService.getToken();
    waitForToken.then((val) {
      print('retrieved token: $val');
    }).catchError((err) => print(err));

    var waitForRememberMeFlag = AuthService.getRememberMeFlag();
    waitForRememberMeFlag.then((val) {
      print('retrieved rememberMeFlag: $val');
    }).catchError((err) => print(err));
  }

  @override
  void dispose() async {
    WidgetsBinding.instance.removeObserver(this); // Remove observer
    if (await AuthService.getToken() != null) {
      if (!await AuthService.getRememberMeFlag()) {
        await AuthService.clearLogin();
      }
    }
    super.dispose();
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) async {
    // App is going to the background
    if (state == AppLifecycleState.paused) {
      print('app is paused');
      // clear local storage data if remember me flag wasnt checked when logging in
      if (await AuthService.getToken() != null) {
        if (!await AuthService.getRememberMeFlag()) {
          await AuthService.clearLogin();
        }
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        title: const Text("Login Page"),
      ),
      body: SingleChildScrollView(
        child: Column(
          children: <Widget>[
            Padding(
              padding: const EdgeInsets.only(top: 60.0),
              child: Center(
                child: SizedBox(
                    width: 200,
                    height: 150,
                    /*decoration: BoxDecoration(
                        color: Colors.red,
                        borderRadius: BorderRadius.circular(50.0)),*/
                    child: Image.asset(
                        'assets/images/password_manager_flaticon.png')),
              ),
            ),
            Padding(
              //padding: const EdgeInsets.only(left:15.0,right: 15.0,top:0,bottom: 0),
              padding: const EdgeInsets.symmetric(horizontal: 15),
              child: TextField(
                controller: emailController,
                decoration: const InputDecoration(
                    border: OutlineInputBorder(),
                    labelText: 'Email',
                    hintText: 'Enter a valid email such as abc@gmail.com'),
              ),
            ),
            Padding(
              padding: const EdgeInsets.only(
                  left: 15.0, right: 15.0, top: 15, bottom: 0),
              //padding: EdgeInsets.symmetric(horizontal: 15),
              child: TextField(
                controller: passwordController,
                obscureText: obscureText,
                decoration: InputDecoration(
                    suffixIcon: IconButton(
                      icon: Icon(
                        obscureText ? Icons.visibility : Icons.visibility_off,
                      ),
                      onPressed: () {
                        obscureText = !obscureText;
                        setState(() {});
                      },
                    ),
                    border: const OutlineInputBorder(),
                    labelText: 'Password',
                    hintText: 'Enter a secure password'),
              ),
            ),
            // Remember Me checkbox
            CheckboxListTile(
              title: const Text("Remember Me"),
              value: rememberMe,
              onChanged: (bool? newValue) {
                rememberMe = newValue ?? false;
                setState(() {});
              },
              controlAffinity: ListTileControlAffinity.leading,
            ),
            const SizedBox(
              height: 30,
            ),
            ElevatedButton(
              onPressed: () {
                //TODO: redirect user to user management system
              },
              child: const Text(
                'Forgot Password',
                style: TextStyle(color: Colors.blue, fontSize: 15),
              ),
            ),
            const SizedBox(
              height: 30,
            ),
            Container(
              height: 50,
              width: 250,
              decoration: BoxDecoration(
                  color: Colors.blue, borderRadius: BorderRadius.circular(20)),
              child: ElevatedButton(
                onPressed: () async {
                  // Navigator.pushNamed(context, NavigationHelperWidget.routeID);
                  print("logging in...");
                  print('${emailController?.text}');
                  print('${passwordController?.text}');

                  var response = await authService.login(
                      emailController?.text ?? "",
                      passwordController?.text ?? "",
                      rememberMe);

                  print("result: ${response?.toJson()}");

                  // if token exists then save login info depending on rememberMe state
                  var value = await AuthService.getToken();
                  // print('value: $value');
                },
                child: const Text(
                  'Login',
                  style: TextStyle(color: Colors.white, fontSize: 25),
                ),
              ),
            ),
            const SizedBox(
              height: 130,
            ),
            const Text('New User? Create Account')
          ],
        ),
      ),
    );
  }
}
