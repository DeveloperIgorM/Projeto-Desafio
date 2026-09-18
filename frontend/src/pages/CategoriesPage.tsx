import { useState } from 'react'
import type { FormEvent } from 'react'
import { Plus, Tag, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { useAuth } from '@/contexts/AuthContext'
import { useCategories, useCreateCategory, useDeleteCategory } from '@/hooks/useCategories'

export function CategoriesPage() {
  const { hasRole } = useAuth()
  const isAdmin = hasRole('admin')

  const { data: categories, isLoading } = useCategories()
  const createCategory = useCreateCategory()
  const deleteCategory = useDeleteCategory()

  const [name, setName] = useState('')
  const [error, setError] = useState<string | null>(null)

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)

    if (!name.trim()) {
      setError('Informe um nome para a categoria.')
      return
    }

    createCategory.mutate(name.trim(), {
      onSuccess: () => setName(''),
      onError: () => setError('Nao foi possivel criar a categoria. Tente novamente.'),
    })
  }

  function handleDelete(id: string) {
    if (!window.confirm('Excluir esta categoria? Essa acao nao pode ser desfeita.')) return
    deleteCategory.mutate(id, {
      onError: () => window.alert('Nao foi possivel excluir. Verifique se ainda ha produtos usando essa categoria.'),
    })
  }

  return (
    <div className="flex flex-col gap-6">
      <div>
        <h1 className="text-2xl font-semibold">Categorias</h1>
        <p className="text-sm text-muted-foreground">Organize os produtos por categoria.</p>
      </div>

      {isAdmin && (
        <Card>
          <CardContent>
            <form onSubmit={handleSubmit} className="flex flex-col gap-3 sm:flex-row sm:items-end">
              <div className="flex-1">
                <label htmlFor="category-name" className="mb-1 block text-sm font-medium">
                  Nova categoria
                </label>
                <Input
                  id="category-name"
                  value={name}
                  onChange={(event) => setName(event.target.value)}
                  placeholder="Ex: Eletronicos"
                  maxLength={100}
                />
              </div>
              <Button type="submit" disabled={createCategory.isPending}>
                <Plus className="size-4" />
                Adicionar
              </Button>
            </form>
            {error && <p className="mt-2 text-sm text-destructive">{error}</p>}
          </CardContent>
        </Card>
      )}

      <Card>
        <CardContent>
          {isLoading ? (
            <p className="text-sm text-muted-foreground">Carregando categorias...</p>
          ) : !categories || categories.length === 0 ? (
            <p className="text-sm text-muted-foreground">Nenhuma categoria cadastrada ainda.</p>
          ) : (
            <ul className="divide-y">
              {categories.map((category) => (
                <li key={category.id} className="flex items-center justify-between py-3">
                  <span className="flex items-center gap-2 text-sm">
                    <Tag className="size-4 text-muted-foreground" />
                    {category.name}
                  </span>
                  {isAdmin && (
                    <button
                      type="button"
                      onClick={() => handleDelete(category.id)}
                      className="rounded-md p-1.5 text-muted-foreground hover:bg-accent hover:text-destructive"
                      aria-label={`Excluir ${category.name}`}
                    >
                      <Trash2 className="size-4" />
                    </button>
                  )}
                </li>
              ))}
            </ul>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
