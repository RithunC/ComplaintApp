import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminUsersComponent } from './admin-users.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
describe('AdminUsersComponent', () => {
  let f: ComponentFixture<AdminUsersComponent>;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports:[AdminUsersComponent,RouterTestingModule,HttpClientTestingModule] }).compileComponents(); f = TestBed.createComponent(AdminUsersComponent); f.detectChanges(); });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('agents empty initially', () => expect(f.componentInstance.agents().length).toBe(0));
});
