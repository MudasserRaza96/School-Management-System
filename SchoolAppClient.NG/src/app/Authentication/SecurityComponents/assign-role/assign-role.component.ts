import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../SecurityModels/auth.service';

export interface UserRoleAssignment {
  userId: string;
  username: string;
  email: string;
  roles: string[];
}

export interface UserRoleTableRow {
  rowId: number;
  userId: string;
  username: string;
  roleName: string;
  status: string;
}

export interface RoleCheckItem {
  name: string;
  checked: boolean;
}

@Component({
  selector: 'app-assign-role',
  templateUrl: './assign-role.component.html',
  styleUrl: './assign-role.component.css'
})
export class AssignRoleComponent implements OnInit {
  // Current active view: 'list' (Table View) or 'assign' (Dual Transfer Box View)
  currentView: 'list' | 'assign' = 'list';

  // Data lists
  allSystemRoles: string[] = [];
  userAssignments: UserRoleAssignment[] = [];
  
  // Table view data
  tableRows: UserRoleTableRow[] = [];
  filteredTableRows: UserRoleTableRow[] = [];
  tableSearchQuery: string = '';
  selectedFilterUserOrRole: string = '';

  // Assign Transfer View data
  selectedUsername: string = '';
  assignedRoles: RoleCheckItem[] = [];
  unassignedRoles: RoleCheckItem[] = [];
  
  searchAssignedQuery: string = '';
  searchUnassignedQuery: string = '';
  
  selectAllAssigned: boolean = false;
  selectAllUnassigned: boolean = false;

  // Status flags & feedback
  isLoading: boolean = false;
  isSaving: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.loadAllData();
  }

  loadAllData(): void {
    this.isLoading = true;
    this.clearMessages();

    // Load system roles
    this.authService.getRoles().subscribe({
      next: (rolesData) => {
        this.allSystemRoles = (rolesData || []).map(r => r.name);
        
        // Load user role assignments
        this.authService.getUserRoles().subscribe({
          next: (userRolesData) => {
            this.userAssignments = userRolesData || [];
            this.buildTableRows();
            this.isLoading = false;
          },
          error: (err) => {
            console.error('Failed to load user roles', err);
            this.errorMessage = 'Failed to load user role assignments.';
            this.isLoading = false;
          }
        });
      },
      error: (err) => {
        console.error('Failed to load system roles', err);
        this.errorMessage = 'Failed to load system roles.';
        this.isLoading = false;
      }
    });
  }

  buildTableRows(): void {
    const rows: UserRoleTableRow[] = [];
    let count = 1;

    for (const assignment of this.userAssignments) {
      if (assignment.roles && assignment.roles.length > 0) {
        for (const role of assignment.roles) {
          rows.push({
            rowId: 1, // as shown in screenshot ID column shows 1 or sequential
            userId: assignment.userId,
            username: assignment.username || assignment.email,
            roleName: role,
            status: 'Active'
          });
        }
      } else {
        rows.push({
          rowId: 1,
          userId: assignment.userId,
          username: assignment.username || assignment.email,
          roleName: 'No Role Assigned',
          status: 'Active'
        });
      }
    }

    this.tableRows = rows;
    this.applyTableFilter();
  }

  applyTableFilter(): void {
    let result = [...this.tableRows];

    if (this.selectedFilterUserOrRole) {
      const selected = this.selectedFilterUserOrRole.toLowerCase();
      result = result.filter(r => 
        r.username.toLowerCase() === selected || 
        r.roleName.toLowerCase() === selected
      );
    }

    if (this.tableSearchQuery.trim()) {
      const q = this.tableSearchQuery.toLowerCase();
      result = result.filter(r => 
        r.username.toLowerCase().includes(q) || 
        r.roleName.toLowerCase().includes(q)
      );
    }

    this.filteredTableRows = result;
  }

  resetTableFilter(): void {
    this.tableSearchQuery = '';
    this.selectedFilterUserOrRole = '';
    this.applyTableFilter();
  }

  // --- View Switcher ---
  openAssignView(username?: string): void {
    this.clearMessages();

    if (username) {
      this.selectedUsername = username;
    } else if (this.userAssignments.length > 0) {
      this.selectedUsername = this.userAssignments[0].username;
    } else {
      this.selectedUsername = '';
    }

    this.setupTransferLists();
    this.currentView = 'assign';
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  switchToList(): void {
    this.currentView = 'list';
    this.loadAllData();
  }

  onUserSelectChange(): void {
    this.setupTransferLists();
  }

  setupTransferLists(): void {
    const userAssignment = this.userAssignments.find(
      u => u.username.toLowerCase() === this.selectedUsername.toLowerCase() ||
           u.email.toLowerCase() === this.selectedUsername.toLowerCase()
    );

    const userRoleNames = userAssignment ? (userAssignment.roles || []) : [];

    // Assigned roles list
    this.assignedRoles = userRoleNames.map(r => ({ name: r, checked: false }));

    // Unassigned roles list
    const unassignedNames = this.allSystemRoles.filter(r => !userRoleNames.includes(r));
    this.unassignedRoles = unassignedNames.map(r => ({ name: r, checked: false }));

    this.searchAssignedQuery = '';
    this.searchUnassignedQuery = '';
    this.selectAllAssigned = false;
    this.selectAllUnassigned = false;
  }

  // Filtered transfer lists
  get visibleAssignedRoles(): RoleCheckItem[] {
    if (!this.searchAssignedQuery.trim()) return this.assignedRoles;
    const q = this.searchAssignedQuery.toLowerCase();
    return this.assignedRoles.filter(r => r.name.toLowerCase().includes(q));
  }

  get visibleUnassignedRoles(): RoleCheckItem[] {
    if (!this.searchUnassignedQuery.trim()) return this.unassignedRoles;
    const q = this.searchUnassignedQuery.toLowerCase();
    return this.unassignedRoles.filter(r => r.name.toLowerCase().includes(q));
  }

  // Toggle All Checkboxes
  toggleSelectAllAssigned(): void {
    const visible = this.visibleAssignedRoles;
    for (const item of visible) {
      item.checked = this.selectAllAssigned;
    }
  }

  toggleSelectAllUnassigned(): void {
    const visible = this.visibleUnassignedRoles;
    for (const item of visible) {
      item.checked = this.selectAllUnassigned;
    }
  }

  // Transfer Actions
  moveUnassignedToAssigned(): void {
    const toMove = this.unassignedRoles.filter(r => r.checked);
    if (toMove.length === 0) return;

    for (const item of toMove) {
      item.checked = false;
      this.assignedRoles.push(item);
    }

    this.unassignedRoles = this.unassignedRoles.filter(r => !toMove.includes(r));
    this.selectAllUnassigned = false;
  }

  moveAssignedToUnassigned(): void {
    const toMove = this.assignedRoles.filter(r => r.checked);
    if (toMove.length === 0) return;

    for (const item of toMove) {
      item.checked = false;
      this.unassignedRoles.push(item);
    }

    this.assignedRoles = this.assignedRoles.filter(r => !toMove.includes(r));
    this.selectAllAssigned = false;
  }

  // Save / Update User Roles
  saveUserRoles(): void {
    if (!this.selectedUsername) {
      this.errorMessage = 'Please select a target user.';
      return;
    }

    this.clearMessages();
    this.isSaving = true;

    const assignedNames = this.assignedRoles.map(r => r.name);

    const payload = {
      username: this.selectedUsername,
      role: assignedNames.length > 0 ? assignedNames[0] : '',
      roles: assignedNames
    };

    this.authService.updateUserRole(payload).subscribe({
      next: (res) => {
        this.isSaving = false;
        this.successMessage = res.message || `Roles updated successfully for user "${this.selectedUsername}"!`;
        setTimeout(() => {
          this.switchToList();
        }, 1200);
      },
      error: (err) => {
        this.isSaving = false;
        console.error('Failed to update roles', err);
        this.errorMessage = err.error?.message || 'Failed to update user roles.';
      }
    });
  }

  // Delete / Remove Single User Role from Table
  deleteRoleRow(row: UserRoleTableRow): void {
    if (row.roleName === 'No Role Assigned') return;

    if (!confirm(`Are you sure you want to remove role "${row.roleName}" from user "${row.username}"?`)) {
      return;
    }

    this.clearMessages();
    this.authService.removeUserRole(row.username, row.roleName).subscribe({
      next: (res) => {
        this.successMessage = res.message || `Role "${row.roleName}" removed successfully from user "${row.username}".`;
        this.loadAllData();
      },
      error: (err) => {
        console.error('Delete role error:', err);
        this.errorMessage = err.error?.message || 'Failed to remove user role.';
      }
    });
  }

  clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }
}
