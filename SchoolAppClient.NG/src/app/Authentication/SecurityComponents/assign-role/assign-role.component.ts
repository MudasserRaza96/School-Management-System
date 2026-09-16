import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../SecurityModels/auth.service';

@Component({
  selector: 'app-assign-role',
  templateUrl: './assign-role.component.html',
  styleUrl: './assign-role.component.css'
})
export class AssignRoleComponent implements OnInit {
  username: string = '';
  selectedRole: string = '';
  roles: any[] = [];
  
  isLoadingRoles: boolean = false;
  isSubmitting: boolean = false;
  
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.fetchRoles();
  }

  fetchRoles(): void {
    this.isLoadingRoles = true;
    this.authService.getRoles().subscribe({
      next: (data) => {
        this.roles = data || [];
        this.isLoadingRoles = false;
      },
      error: (err) => {
        console.error('Failed to load roles list', err);
        this.errorMessage = 'Failed to load roles dropdown list.';
        this.isLoadingRoles = false;
      }
    });
  }

  assignRole(): void {
    if (!this.username || !this.username.trim()) {
      this.errorMessage = 'Please enter a valid Username or Email.';
      return;
    }

    if (!this.selectedRole) {
      this.errorMessage = 'Please select a role to assign.';
      return;
    }

    this.clearMessages();
    this.isSubmitting = true;

    const payload = {
      username: this.username.trim(),
      role: this.selectedRole
    };

    this.authService.assignRole(payload).subscribe({
      next: (res) => {
        this.isSubmitting = false;
        this.successMessage = res.message || `Role "${this.selectedRole}" assigned successfully to user "${this.username}"!`;
        // Optionally reset form
        this.username = '';
        this.selectedRole = '';
      },
      error: (err) => {
        this.isSubmitting = false;
        console.error('Role assignment error:', err);
        if (err.error && typeof err.error === 'object') {
          const errorsObj = err.error;
          const firstKey = Object.keys(errorsObj)[0];
          if (firstKey && Array.isArray(errorsObj[firstKey])) {
            this.errorMessage = errorsObj[firstKey][0];
            return;
          }
        }
        this.errorMessage = err.error?.message || 'Failed to assign role. Please verify the username/email and try again.';
      }
    });
  }

  clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }
}
