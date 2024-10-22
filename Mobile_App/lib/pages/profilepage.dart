import 'package:flutter/material.dart';
import 'package:PasswordManager/Models/ClaimTypes.dart';

import 'package:PasswordManager/Models/LoginDTO.dart';
import 'package:PasswordManager/Services/AuthService.dart';

class ProfilePage extends StatelessWidget {
  const ProfilePage({
    super.key,
    LoginDTO? loginDTO,
  });

  Future<Map<String, dynamic>> getClaims() async {
    var claims = await AuthService.getClaims();
    return claims;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Your Profile Information'),
        automaticallyImplyLeading: false,
      ),
      body: FutureBuilder<Map<String, dynamic>>(
        future: getClaims(), // Call your async function here
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            // While the data is being loaded
            return const Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            // If there's an error
            return Center(child: Text('Error: ${snapshot.error}'));
          } else if (snapshot.hasData) {
            // Once data is available
            final claims = snapshot.data!;
            final email = claims[ClaimTypes.email];
            final role = claims[ClaimTypes.role];
            final userId = claims[ClaimTypes.nameIdentifier];
            final name = claims[ClaimTypes.name];

            return Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: <Widget>[
                  Text(
                    'Name: $name',
                    style: const TextStyle(fontSize: 24.0), // Set your desired font size here
                  ),
                  Text(
                    'Email: $email',
                    style: const TextStyle(fontSize: 24.0), // Set your desired font size here
                  ),
                  Text(
                    'Role: $role',
                    style: const TextStyle(fontSize: 24.0), // Set your desired font size here
                  ),
                  Text(
                    'User ID: $userId',
                    style: const TextStyle(fontSize: 24.0), // Set your desired font size here
                  ),
                ],
              ),
            );
          } else {
            // Fallback for any other case
            return const Center(child: Text('No data available.'));
          }
        },
      ),
    );
  }
}
