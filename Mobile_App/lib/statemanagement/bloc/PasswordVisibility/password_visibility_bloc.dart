import 'package:PasswordManager/statemanagement/bloc/PasswordVisibility/password_visibility_event.dart';
import 'package:PasswordManager/statemanagement/bloc/PasswordVisibility/password_visibility_state.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

class PasswordVisibilityBloc
    extends Bloc<PasswordVisibilityEvent, PasswordVisibilityState> {
  PasswordVisibilityBloc() : super(PasswordVisibilityState(false)) {
    on<PasswordVisibilityToggle>(
        (event, emit) => emit(PasswordVisibilityState(!state.isVisible)));
  }
}
