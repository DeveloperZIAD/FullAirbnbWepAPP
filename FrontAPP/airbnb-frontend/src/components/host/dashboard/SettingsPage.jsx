export default function SettingsPage() {
  return (
    <div className="space-y-6 animate-in fade-in duration-500">
      {/* Header */}
      <header>
        <h1 className="text-3xl font-extrabold text-neutral-800">Settings</h1>
        <p className="text-neutral-500 mt-1">
          Manage your account preferences, profile, and security here.
        </p>
      </header>

      {/* Settings Content Area */}
      <div className="bg-white p-8 rounded-xl border shadow-sm">
        <h3 className="text-lg font-semibold mb-4 text-neutral-800">
          Profile Settings
        </h3>
        <p className="text-neutral-600">
          This is where you can update your host details and notification
          preferences.
        </p>
        {/* يمكنك إضافة مكونات مثل ProfileForm أو PasswordUpdate هنا لاحقاً */}
      </div>
    </div>
  );
}
