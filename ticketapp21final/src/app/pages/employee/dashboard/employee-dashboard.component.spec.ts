import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmployeeDashboardComponent } from './employee-dashboard.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
describe('EmployeeDashboardComponent', () => {
  let f: ComponentFixture<EmployeeDashboardComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [EmployeeDashboardComponent, RouterTestingModule, HttpClientTestingModule] }).compileComponents();
    f = TestBed.createComponent(EmployeeDashboardComponent);
    f.detectChanges();
  });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('summary null initially', () => expect(f.componentInstance.summary()).toBeNull());
});
