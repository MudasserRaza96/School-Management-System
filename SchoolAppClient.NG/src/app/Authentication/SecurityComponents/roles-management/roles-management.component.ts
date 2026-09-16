import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../SecurityModels/auth.service';

export interface UserRole {
  id?: string;
  name: string;
}

@Component({
  selector: 'app-roles-management',
  templateUrl: './roles-management.component.html',
  styleUrl: './roles-management.component.css'
})
export class RolesManagementComponent implements OnInit {
  roles: UserRole[] = [];
  filteredRoles: UserRole[] = [];
  searchQuery: string = '';
  isLoading: boolean = false;
  
  // Modal / Form state
  showModal: boolean = false;
  isEditMode: boolean = false;
  currentRole: UserRole = { name: '' };
  
  // Feedback messages
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles(): void {
    this.isLoading = true;
    this.authService.getRoles().subscribe({
      next: (data) => {
        this.roles = data || [];
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load roles', err);
        this.errorMessage = 'Failed to load roles list.';
        this.isLoading = false;
      }
    });
  }

  applyFilter(): void {
    if (!this.searchQuery.trim()) {
      this.filteredRoles = [...this.roles];
      return;
    }
    const q = this.searchQuery.toLowerCase();
    this.filteredRoles = this.roles.filter(r => 
      (r.name && r.name.toLowerCase().includes(q)) || 
      (r.id && r.id.toLowerCase().includes(q))
    );
  }

  openCreateModal(): void {
    this.isEditMode = false;
    this.currentRole = { name: '' };
    this.clearMessages();
    this.showModal = true;
  }

  openEditModal(role: UserRole): void {
    this.isEditMode = true;
    this.currentRole = { ...role };
    this.clearMessages();
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.currentRole = { name: '' };
  }

  saveRole(): void {
    if (!this.currentRole.name || !this.currentRole.name.trim()) {
      this.errorMessage = 'Role name cannot be empty.';
      return;
    }

    this.clearMessages();

    if (this.isEditMode && this.currentRole.id) {
      this.authService.updateRole(this.currentRole.id, { name: this.currentRole.name }).subscribe({
        next: (res) => {
          this.successMessage = res.message || 'Role updated successfully!';
          this.closeModal();
          this.loadRoles();
        },
        error: (err) => {
          console.error(err);
          this.errorMessage = err.error?.message || 'Failed to update role.';
        }
      });
    } else {
      this.authService.createRole({ name: this.currentRole.name }).subscribe({
        next: (res) => {
          this.successMessage = 'Role created successfully!';
          this.closeModal();
          this.loadRoles();
        },
        error: (err) => {
          console.error(err);
          this.errorMessage = err.error?.message || 'Failed to create role.';
        }
      });
    }
  }

  deleteRole(role: UserRole): void {
    if (!role.id) return;
    if (!confirm(`Are you sure you want to delete the role "${role.name}"?`)) return;

    this.clearMessages();
    this.authService.deleteRole(role.id).subscribe({
      next: (res) => {
        this.successMessage = res.message || 'Role deleted successfully!';
        this.loadRoles();
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.error?.message || 'Failed to delete role.';
      }
    });
  }

  clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }
}
