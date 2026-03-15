# Testing Guide - Delete Fix

## Quick Test (2 minutes)

### Step 1: Start the Application
```bash
# In your terminal (from BlazorApp directory):
dotnet run
```

### Step 2: Navigate to Dashboard
1. Open your browser to `https://localhost:5001` (or your configured port)
2. Click on "Candidates" or navigate to `/dynamics/dashboard`

### Step 3: Test Delete Functionality

#### Test Case A: Delete a Submission
1. **View the Dashboard** - You should see a list of submissions
2. **Locate a submission** - Pick any submission in the table
3. **Click Delete button** - Find the "Delete" link in the Actions column
4. **Confirm deletion** - A confirmation modal should appear
5. **Click Delete in modal** - Click the red "Delete" button
6. **Verify result** - The submission should disappear from the list
   - ✅ Success: No error message, submission removed
   - ❌ Failure: Would show "Operation is not valid..." error

#### Test Case B: Test Multiple Deletions
1. Delete 2-3 different submissions in succession
2. Verify each deletion works without errors
3. ✅ Success: All deletions work smoothly

#### Test Case C: Verify Data Integrity
1. Create a new submission (go to "Add Candidate")
2. Fill in some data and save
3. Go back to Dashboard
4. Delete the newly created submission
5. ✅ Success: New submission is successfully deleted

## Browser Console Check

While testing, monitor the browser console for any JavaScript errors:

1. **Open Developer Tools** - Press `F12` in browser
2. **Go to Console tab** - Click on "Console"
3. **Perform delete operation** - Try to delete a submission
4. **Check for errors** - Should see no errors in red
   - ✅ Clean console (no red errors)
   - ❌ Any red error messages indicate a problem

## Server Log Check

Monitor the server logs for any errors:

1. **Watch the terminal** where `dotnet run` is executing
2. **Look for errors** related to delete operations
3. **Check for the debug message** if an error occurs:
   ```
   Delete failed for id 'xxx': [error message]
   ```

## Network Inspector Check

Verify the API requests are working correctly:

1. **Open Browser DevTools** - Press `F12`
2. **Go to Network tab** - Click "Network"
3. **Perform delete operation**
4. **Look for the request** - Should see a DELETE or POST request to `/api/submissions/{id}`
5. **Check response status**:
   - ✅ 200 or 204 = Success
   - ❌ 400, 500 = Error

## Common Issues & Solutions

### Issue 1: "Operation is not valid due to the current state of the object"
**Status**: ✅ **FIXED** - This error should no longer occur

### Issue 2: Delete button seems to work but submission still appears
**Possible Cause**: Cache not invalidated properly
**Solution**:
1. Hard refresh browser: `Ctrl+F5` (Windows) or `Cmd+Shift+R` (Mac)
2. Clear browser cache
3. Restart the application

### Issue 3: "Partition key path not found in document"
**Possible Cause**: Data corruption in Cosmos DB
**Solution**:
1. Check if the submission data is valid
2. Verify the partition key is present in the document
3. Contact database administrator

### Issue 4: Delete is slow (takes several seconds)
**Expected Behavior**: This is normal
- First delete from cold start: 2-3 seconds (Cosmos SDK initialization)
- Subsequent deletes: <1 second
**No Action Needed** - This is expected behavior

## Rollback (If Needed)

If you need to revert to the previous code:

```bash
# Restore the original DeleteAsync method
git checkout HEAD -- BlazorApp/Repositories/Implementations/FormSubmissionCosmosRepository.cs

# Rebuild
dotnet build

# Restart the app
dotnet run
```

## Success Criteria

✅ **The fix is successful if:**
- [x] Delete button works without errors
- [x] Submission is removed from the list immediately
- [x] No "Operation is not valid" error appears
- [x] No JavaScript errors in console
- [x] No exception in server logs
- [x] New submissions can be created and deleted
- [x] Multiple deletions in succession work correctly

## Next Steps

If all tests pass:
1. **Commit the changes** - `git add . && git commit -m "Fix: Delete operation in Dashboard"`
2. **Push to repository** - `git push origin blazor/app`
3. **Update your team** - Let them know the delete bug is fixed

## Need Help?

If you encounter issues:
1. **Check the DELETE_FIX_EXPLANATION.md** - Detailed technical explanation
2. **Review the error message** - Copy the full error text
3. **Check Application Insights** (if configured) - Look for exception details
4. **Check Azure Cosmos DB logs** - Review activity logs for the container

