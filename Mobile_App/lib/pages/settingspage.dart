import 'package:flutter/material.dart';
import 'package:PasswordManager/Models/LoginDTO.dart';

class SettingsPage extends StatelessWidget {
  const SettingsPage({
    super.key,
    LoginDTO? loginDTO,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.all(20),
      padding: const EdgeInsets.all(16),
      child: const Center(
        child: Text("Email me at vincenteri321@gmail.com to request to delete your account",
        style: TextStyle(fontSize: 24.0),),
      ),
    );
  }
}
