import { test, expect } from '@playwright/test';

test.describe('Reference Data MFE', () => {
  test('should redirect to claim-statuses by default', async ({ page }) => {
    await page.goto('/');
    await expect(page).toHaveURL(/.*claim-statuses/);
    
    const table = page.locator('table');
    await expect(table).toBeVisible();
  });

  test('should navigate to policy-types', async ({ page }) => {
    await page.goto('/policy-types');
    await expect(page).toHaveURL(/.*policy-types/);
    
    const table = page.locator('table');
    await expect(table).toBeVisible();
  });

  test('should navigate to coverage-types', async ({ page }) => {
    await page.goto('/coverage-types');
    await expect(page).toHaveURL(/.*coverage-types/);
    
    const table = page.locator('table');
    await expect(table).toBeVisible();
  });

  test('should navigate to regions', async ({ page }) => {
    await page.goto('/regions');
    await expect(page).toHaveURL(/.*regions/);
    
    const table = page.locator('table');
    await expect(table).toBeVisible();
  });

    test('should create a new claim statuses', async({page})=>{
    await page.goto('/claim-statuses/');
    const button = page.getByText('Create new Claim Status');
    await button.click();

    const uniqueCode = `HELLO_${Date.now()}`;

    const code = page.getByLabel('Code');
    await code.fill(uniqueCode);
    
    const name = page.getByLabel('Name');
    await name.fill('TEST HELLO');
    
    const description = page.getByLabel('Description');
    await description.fill('TEST DESCRIPTION');

    const submit = page.getByTestId('submit');
    await submit.click();

    await expect(page.getByRole('cell', { name: uniqueCode, exact: true })).toBeVisible();
  });

  test('should change status to inactive on claim status page', async({page})=>{
    await page.goto('/claim-statuses/');
    const button = page.getByText('Create new Claim Status');
    await button.click();

    const uniqueCode = `DEL_${Date.now()}`;

    const code = page.getByLabel('Code');
    await code.fill(uniqueCode);
    
    const name = page.getByLabel('Name');
    await name.fill('TO DELETE');
    
    const description = page.getByLabel('Description');
    await description.fill('THIS WILL BE DELETED');

    const submit = page.getByTestId('submit');
    await submit.click();

    const row = page.getByRole('row', { name: uniqueCode });
    await expect(row).toBeVisible();

    const deleteButton = row.getByRole('button', { name: 'Delete' });
    await deleteButton.click();

    const confirmButton = page.getByRole('button', { name: 'Confirm' });
    await confirmButton.click();

    await expect(row).not.toBeVisible();
  });

  test('should edit a claim status', async ({page})=>{
    await page.goto('/claim-statuses/');
    const button = page.getByText('Create new Claim Status');
    await button.click();

    const uniqueCode = `EDIT_${Date.now()}`;

    const code = page.getByLabel('Code');
    await code.fill(uniqueCode);
    
    const name = page.getByLabel('Name');
    await name.fill('TO EDIT');
    
    const description = page.getByLabel('Description');
    await description.fill('THIS WILL BE EDIT');

    const submit = page.getByTestId('submit');
    await submit.click();

    const row=page.getByRole('row', {name:uniqueCode});
    await expect(row).toBeVisible();

    const editButton=row.getByRole('link', {name:'Edit'});
    await editButton.click();

    const nameEdit= page.getByLabel('Name');
    await expect(nameEdit).toHaveValue('TO EDIT');
    
    await nameEdit.fill(uniqueCode+'_EDIT');

    const saveButton=page.getByTestId('submit');
    await saveButton.click();

    const rowEdited=page.getByRole('row',{name: uniqueCode+'_EDIT'})
    await expect(rowEdited).toBeVisible();
  });

  test('should show innactive', async({page})=>{
    await page.goto('/claim-statuses/');

    await page.getByText('Show Inactive').click();
    
    await expect(page.getByRole('cell', { name: 'Inactive'}).first()).toBeVisible();
  });
});