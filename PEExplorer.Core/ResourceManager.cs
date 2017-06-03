﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PEExplorer.Core {

    public sealed class ResourceManager : IDisposable {
        readonly IntPtr _hModule;

        public ResourceManager(string path) {
            _hModule = Win32.LoadLibraryEx(path, IntPtr.Zero, Win32.LoadLibraryFlags.LoadAsDataFile | Win32.LoadLibraryFlags.LoadAsImageResource);
            if(_hModule == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        void Dispose(bool disposing) {
            Win32.FreeLibrary(_hModule);
            if(disposing)
                GC.SuppressFinalize(this);
        }

        public void Dispose() {
            Dispose(true);
        }

        ~ResourceManager() {
            Dispose(false);
        }

        readonly StringBuilder _text = new StringBuilder(2048);

        public string GetResourceString(ResourceID id) {
            Win32.LoadString(_hModule, id.Id, _text, _text.Capacity);
            return _text.ToString();
        }

        public ICollection<StringResource> GetStringTableContent(ResourceID name) {
            var content = GetResourceContent(name, ResourceID.StringTable);
            var index = 0;
            var strings = new List<StringResource>(8);
            for(int i = 0; i < 16; i++) {       // 16 is max strings in string table
                var length = BitConverter.ToInt16(content, index);
                index += 2;
                if(length == 0)
                    continue;


                strings.Add(new StringResource {
                    Id = i,
                    Value = Encoding.Unicode.GetString(content, index, length * 2)
                });
                index += length * 2;
            }

            return strings;
        }

        public unsafe ICollection<ResourceID> GetResourceTypes() {
            var types = new List<ResourceID>();
            Win32.EnumResourceTypes(_hModule, (handle, ptr, param) => {
                if((long)ptr < 0x10000)
                    types.Add(new ResourceID((int)ptr));
                else
                    types.Add(new ResourceID(new string((char*)ptr)));
                return true;
            }, IntPtr.Zero);
            return types;
        }

        public unsafe ICollection<ResourceID> GetResourceNames(ResourceID type) {
            var names = new List<ResourceID>();
            Win32.EnumResNameProc proc = (module, typeName, name, param) => {
                if((long)name < 0x10000)
                    names.Add(new ResourceID((int)name));
                else
                    names.Add(new ResourceID(new string((char*)name)));
                return true;
            };

            if(type.IsId)
                Win32.EnumResourceNames(_hModule, new IntPtr(type.Id), proc, IntPtr.Zero);
            else
                Win32.EnumResourceNames(_hModule, type.Name, proc, IntPtr.Zero);
            return names;
        }

        public byte[] GetResourceContent(ResourceID name, ResourceID type) {
            IntPtr iname, itype;
            using(var d1 = name.GetAsIntPtr(out iname)) {
                using(var d2 = type.GetAsIntPtr(out itype)) {
                    var hResource = Win32.FindResource(_hModule, iname, itype);
                    if(hResource == IntPtr.Zero) return null;

                    var hGlobal = Win32.LoadResource(_hModule, hResource);
                    if(hGlobal == IntPtr.Zero) return null;

                    var size = Win32.SizeofResource(_hModule, hResource);
                    var ptr = Win32.LockResource(hGlobal);

                    var buffer = new byte[size];
                    Marshal.Copy(ptr, buffer, 0, buffer.Length);
                    return buffer;
                }
            }
        }

        public ImageSource GetIconImage(ResourceID id, bool icon) {
            IntPtr iconName;
            using(var d = id.GetAsIntPtr(out iconName)) {
                var hIcon = Win32.LoadImage(_hModule, iconName, icon ? Win32.ImageType.Icon : Win32.ImageType.Cursor, 0, 0, Win32.LoadImageFlags.None);
                if(hIcon == IntPtr.Zero) {
                    var bytes = GetResourceContent(id, icon ? ResourceID.Icon : ResourceID.Cursor);
                    hIcon = Win32.CreateIconFromResource(bytes, bytes.Length, icon, 0x30000);
                    if(hIcon == IntPtr.Zero)
                        return null;
                }

                var source = Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                if(icon)
                    Win32.DestroyIcon(hIcon);
                else
                    Win32.DestroyCursor(hIcon);

                return source;
            }
        }

        public ImageSource GetBitmapImage(ResourceID id) {
            IntPtr bitmapName;
            using(var d = id.GetAsIntPtr(out bitmapName)) {
                var hBitmap = Win32.LoadImage(_hModule, bitmapName, Win32.ImageType.Bitmap, 0, 0, Win32.LoadImageFlags.CreateDibSection);
                if(hBitmap == IntPtr.Zero) return null;

                var source = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                Win32.DeleteObject(hBitmap);
                return source;
            }
        }
    }
}
