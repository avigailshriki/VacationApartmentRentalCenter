import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Addproprty } from './addproprty';

describe('Addproprty', () => {
  let component: Addproprty;
  let fixture: ComponentFixture<Addproprty>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Addproprty]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Addproprty);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
