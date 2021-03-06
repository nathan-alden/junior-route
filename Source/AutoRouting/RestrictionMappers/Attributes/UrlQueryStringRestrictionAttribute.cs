﻿using System;

using Junior.Common;
using Junior.Route.AutoRouting.Containers;

namespace Junior.Route.AutoRouting.RestrictionMappers.Attributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class UrlQueryStringRestrictionAttribute : RestrictionAttribute
	{
		private readonly string _field;
		private readonly RequestValueComparer? _fieldComparer;
		private readonly bool _optional;
		private readonly string _value;
		private readonly RequestValueComparer? _valueComparer;

		public UrlQueryStringRestrictionAttribute(string field, string value, bool optional = false)
		{
			field.ThrowIfNull("field");
			value.ThrowIfNull("value");

			_field = field;
			_value = value;
			_optional = optional;
		}

		public UrlQueryStringRestrictionAttribute(string field, RequestValueComparer fieldComparer, string value, RequestValueComparer valueComparer, bool optional = false)
		{
			field.ThrowIfNull("field");
			value.ThrowIfNull("value");

			_field = field;
			_fieldComparer = fieldComparer;
			_value = value;
			_valueComparer = valueComparer;
			_optional = optional;
		}

		public override void Map(Routing.Route route, IContainer container)
		{
			route.ThrowIfNull("route");
			container.ThrowIfNull("container");

			if (_fieldComparer != null && _valueComparer != null)
			{
				route.RestrictByUrlQueryString(_field, GetComparer(_fieldComparer.Value), _value, GetComparer(_valueComparer.Value), _optional);
			}
			else
			{
				route.RestrictByUrlQueryString(_field, _value, _optional);
			}
		}
	}
}