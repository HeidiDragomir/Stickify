import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { passwordValidator } from '../../../../core/validators/password.validator.js';
import { Authentication } from '../../services/authentication.js';
import { HotToastService } from '@ngxpert/hot-toast';
import { Router } from '@angular/router';
import { LoginFormState } from '../../types/login-form-state.js';

@Component({
    selector: 'app-login',
    imports: [ReactiveFormsModule],
    templateUrl: './login.html',
    styleUrl: './login.css',
})
export class Login {
    // Inject the authentication service
    private readonly authService = inject(Authentication);
    readonly toast = inject(HotToastService);
    private readonly router = inject(Router);

    // Create a initial state for form
    readonly formState = signal<LoginFormState>({
        isLoading: false,
        isSubmitted: false,
        errorMessage: '',
    });

    // Define a reactive form with two form controls: email and password
    // Each control has validators that enforce rules
    loginForm = new FormGroup({
        email: new FormControl<string>('', [Validators.required, Validators.email]),
        password: new FormControl<string>('', [Validators.required, passwordValidator]),
    });

    // Convenient getter for the email control — makes it easier to use in the template
    get email() {
        return this.loginForm.get('email');
    }

    // Convenient getter for the password control
    get password() {
        return this.loginForm.get('password');
    }

    // Triggered when the user submits the form
    onSubmit() {
        this.formState.update((state) => ({ ...state, isLoading: true }));

        // Check if form is valid
        // If the form is invalid, mark all fields as "touched"
        // so that validation messages become visible in the UI,
        // then stop execution.
        if (this.loginForm.invalid) {
            this.loginForm.markAllAsTouched();
            this.formState.update((state) => ({ ...state, isLoading: false }));
            return;
        }

        // Extract the form values into a plain object.
        // The "as" cast ensures the object matches the expected type.
        const credentials = this.loginForm.value as { email: string; password: string };

        // Call the Authentication service's `login()` method,
        // which returns an Observable<LoginResponse>.
        // Subscribe to handle both success and error cases.
        this.authService.login(credentials).subscribe({
            // Called when the HTTP request succeeds (status 200)
            next: (response) => {
                console.log('Login successful', response);

                localStorage.setItem('accessToken', response.accessToken);
                localStorage.setItem('user', JSON.stringify(response.user));

                this.loginForm.reset();

                this.formState.set({ isLoading: false, isSubmitted: true, errorMessage: '' });

                this.toast.success('You are logged in.', {
                    duration: 5000,
                    position: 'top-center',
                });

                this.router.navigate(['/']);
            },

            // Called when the HTTP request fails
            error: (err) => {
                console.log('Login failed.', err);
                this.toast.error('Email or password is incorrect.', {
                    duration: 5000,
                    position: 'top-center',
                });
                this.formState.update((state) => ({
                    ...state,
                    isLoading: false,
                    isSubmitted: false,
                }));
            },
        });
    }

    ngOnInit() {
        console.log(this.loginForm);

        // Subscribe to valueChanges — this emits every time the user types in a field.
        // Useful for debugging or dynamic validation.
        this.loginForm.valueChanges.subscribe((value) => {
            // Clear previous error when user modifies the form
            this.formState.update((state) => ({ ...state, errorMessage: '' }));
        });
    }
}
