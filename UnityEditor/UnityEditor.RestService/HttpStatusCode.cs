using System;

namespace UnityEditor.RestService
{
	internal enum HttpStatusCode
	{
		Ok = 200,
		Created,
		Accepted,
		NoContent = 204,
		BadRequest = 400,
		Forbidden = 403,
		NotFound,
		MethodNotAllowed,
		InternalServerError = 500,
		NotImplemented
	}
}
