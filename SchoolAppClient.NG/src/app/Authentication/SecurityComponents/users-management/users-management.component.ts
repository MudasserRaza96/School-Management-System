import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../SecurityModels/auth.service';

export interface UserItem {
  id?: string;
  username: string;
  email: string;
  role?: string[];
  password?: string;
}

@Component({
  selector: 'app-users-management',
  templateUrl: './users-management.component.html',
  styleUrl: './users-management.component.css'
})
export class UsersManagementComponent implements OnInit {
  users: UserItem[] = [];
  filteredUsers: UserItem[] = [];
  allSystemRoles: any[] = [];

  searchQuery: string = '';
  isLoading: boolean = false;
  isSaving: boolean = false;

  // Modal / Form state
  showModal: boolean = false;
  isEditMode: boolean = false;
  currentUser: UserItem = { username: '', email: '', password: '', role: [] };
  selectedRoleName: string = '';

  // Banners
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoles();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.authService.getUsers().subscribe({
      next: (data) => {
        this.users = data || [];
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load users list', err);
        this.errorMessage = 'Failed to load users.';
        this.isLoading = false;
      }
    });
  }

  loadRoles(): void {
    this.authService.getRoles().subscribe({
      next: (data) => {
        this.allSystemRoles = data || [];
      },
      error: (err) => {
        console.error('Failed to load roles list', err);
      }
    });
  }

  applyFilter(): void {
    if (!this.searchQuery.trim()) {
      this.filteredUsers = [...this.users];
      return;
    }
    const q = this.searchQuery.toLowerCase();
    this.filteredUsers = this.users.filter(u =>
      (u.username && u.username.toLowerCase().includes(q)) ||
      (u.email && u.email.toLowerCase().includes(q)) ||
      (u.role && u.role.some(r => r.toLowerCase().includes(q)))
    );
  }

  openCreateModal(): void {
    this.isEditMode = false;
    this.currentUser = { username: '', email: '', password: '', role: [] };
    this.selectedRoleName = '';
    this.clearMessages();
    this.showModal = true;
  }

  openEditModal(user: UserItem): void {
    this.isEditMode = true;
    this.currentUser = { ...user, password: '' };
    this.selectedRoleName = (user.role && user.role.length > 0) ? user.role[0] : '';
    this.clearMessages();
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.currentUser = { username: '', email: '', password: '', role: [] };
    this.selectedRoleName = '';
  }

  saveUser(): void {
    if (!this.currentUser.username || !this.currentUser.username.trim()) {
      this.errorMessage = 'Username is required.';
      return;
    }
    if (!this.currentUser.email || !this.currentUser.email.trim()) {
      this.errorMessage = 'Email is required.';
      return;
    }
    if (!this.isEditMode && (!this.currentUser.password || !this.currentUser.password.trim())) {
      this.errorMessage = 'Password is required for new user.';
      return;
    }

    this.clearMessages();
    this.isSaving = true;

    const roleArray = this.selectedRoleName ? [this.selectedRoleName] : [];

    if (this.isEditMode && this.currentUser.id) {
      const updatePayload = {
        username: this.currentUser.username.trim(),
        email: this.currentUser.email.trim(),
        role: roleArray,
        password: this.currentUser.password?.trim() || undefined
      };

      this.authService.updateUser(this.currentUser.id, updatePayload).subscribe({
        next: (res) => {
          this.isSaving = false;
          this.successMessage = res.message || `User "${this.currentUser.username}" updated successfully!`;
          this.closeModal();
          this.loadUsers();
        },
        error: (err) => {
          this.isSaving = false;
          console.error(err);
          this.errorMessage = err.error?.message || 'Failed to update user.';
        }
      });
    } else {
      const registerPayload = {
        username: this.currentUser.username.trim(),
        email: this.currentUser.email.trim(),
        password: this.currentUser.password?.trim() || '',
        role: roleArray
      };

      this.authService.register(registerPayload).subscribe({
        next: (res) => {
          this.isSaving = false;
          this.successMessage = `User "${this.currentUser.username}" created successfully!`;
          this.closeModal();
          this.loadUsers();
        },
        error: (err) => {
          this.isSaving = false;
          console.error(err);
          this.errorMessage = err.error?.message || 'Failed to create user.';
        }
      });
    }
  }

  deleteUser(user: UserItem): void {
    if (!user.id) return;
    if (!confirm(`Are you sure you want to delete user "${user.username}"?`)) return;

    this.clearMessages();
    this.authService.deleteUser(user.id).subscribe({
      next: (res) => {
        this.successMessage = res.message || `User "${user.username}" deleted successfully!`;
        this.loadUsers();
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.error?.message || 'Failed to delete user.';
      }
    });
  }

  clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }
}
