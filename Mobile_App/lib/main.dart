import 'package:flutter/material.dart';
import 'package:mobile_app/Models/LoginDTO.dart';
import 'package:mobile_app/navigator.dart';
import 'package:mobile_app/pages/loginpage.dart';

const String appName = "PasswordManager";

void main() {
  runApp(AppHome());
}

class AppHome extends StatelessWidget {
  LoginDTO? loginDTO;

  AppHome({super.key});

  void updateAccount(LoginDTO loginDTO) {
    this.loginDTO = loginDTO;
  }

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: appName,
      theme: ThemeData(
        primarySwatch: Colors.teal,
        visualDensity: VisualDensity.adaptivePlatformDensity,
      ),
      initialRoute: LoginPage.routeID,
      routes: {
        LoginPage.routeID: (ctx) => LoginPage(appName: appName, appHome: this),
        NavigationHelperWidget.routeID: (ctx) {
          return NavigationHelperWidget(loginDTO);
        }
      },
    );
  }
}
