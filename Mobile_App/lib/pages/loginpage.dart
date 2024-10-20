import 'package:flutter/material.dart';
import 'package:mobile_app/Models/LoginDTO.dart';
import 'package:mobile_app/main.dart';

class LoginPage extends StatelessWidget {
  static const String routeID = "/login";
  const LoginPage({
    super.key,
    String appName = "",
    AppHome? appHome,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.all(20),
      padding: const EdgeInsets.all(16), 
      child: const Center(
        child: Text("This is the login page haha"),
      ),
    );
  }
}
