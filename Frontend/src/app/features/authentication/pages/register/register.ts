import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { passwordValidator } from '../../../../core/validators/password.validator.js';
import { Authentication } from '../../services/authentication.js';
import { HotToastService } from '@ngxpert/hot-toast';
import { Router } from '@angular/router';
import { RegisterFormState } from '../../types/register-form-state.js';
import { matchPasswordValidator } from '../../../../core/validators/match-password.validator.js';

@Component({
    selector: 'app-register',
    imports: [ReactiveFormsModule],
    templateUrl: './register.html',
    styleUrl: './register.css',
})
export class Register {
    private readonly authService = inject(Authentication);
    readonly toast = inject(HotToastService);
    private readonly router = inject(Router);

    // Create a initial state for form
    readonly formState = signal<RegisterFormState>({
        isLoading: false,
        isSubmitted: false,
        isRegistrationCompleted: false,
        passwordsMatch: false,
    });

    // Define a reactive form with 4 form controls: userName, email, pasword, confirmPassword
    // Each control has validators that enforce rules.

    registerForm = new FormGroup(
        {
            userName: new FormControl<string>('', [Validators.required, Validators.minLength(3)]),
            email: new FormControl<string>('', [Validators.required, Validators.email]),
            password: new FormControl<string>('', [Validators.required, passwordValidator]),
            confirmPassword: new FormControl<string>('', [Validators.required]),
        },
        {
            validators: matchPasswordValidator,
        }
    );

    // Getters for all controlls - make it easier to use in the template
    get userName() {
        return this.registerForm.get('userName');
    }

    get email() {
        return this.registerForm.get('email');
    }

    get password() {
        return this.registerForm.get('password');
    }

    get confirmPassword() {
        return this.registerForm.get('confirmPassword');
    }

    onSubmit() {
        if (this.registerForm.invalid) {
            this.registerForm.markAllAsTouched();
            return;
        }

        this.formState.update((state) => ({ ...state, isLoading: true }));

        // Extract all the form values into an object
        const credentials = this.registerForm.value as {
            userName: string;
            email: string;
            password: string;
            confirmPassword: string;
        };

        this.authService.register(credentials).subscribe({
            next: (response) => {
                console.log('Register successful', response);

                this.registerForm.reset();

                this.toast.success('You are registered.', {
                    duration: 5000,
                    position: 'top-center',
                });

                this.formState.update((state) => ({
                    ...state,
                    isLoading: false,
                    isRegistrationCompleted: true,
                    isSubmitted: true,
                }));

                this.router.navigate(['/auth/login']);
            },
            error: (err) => {
                this.toast.error('Registration failed. Please try again.', {
                    duration: 5000,
                    position: 'top-center',
                });
                this.formState.update((state) => ({
                    ...state,
                    isLoading: false,
                    isRegistrationCompleted: false,
                    isSubmitted: false,
                }));
            },
        });
    }

    ngOnInit() {
        console.log(this.registerForm);
    }
}
